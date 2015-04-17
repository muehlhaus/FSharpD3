﻿#r "PresentationCore.dll"
#r "PresentationFramework.dll"
#r "WindowsBase.dll"
#r "UIAutomationTypes.dll"
#r "System.Xaml.dll"

//#I "D:/Development/Source/FSharpD3/FSharpD3/bin/Debug/"
//#I "C:/Users/Timo/Development/FSharpD3/FSharpD3/bin/Debug/"

#r "D:/Development/Source/FSharpD3/bin/FunScript.dll"
#r "D:/Development/Source/FSharpD3/bin/FunScript.Interop.dll"
#r "D:/Development/Source/FSharpD3/bin/FunScript.TypeScript.Binding.lib.dll"
#r "D:/Development/Source/FSharpD3/bin/FunScript.TypeScript.Binding.d3.dll"
//#r "D:/Development/Source/FSharpD3/bin/FunScript.TypeScript.Binding.jquery.dll"



#r "D:/Development/Source/FSharpD3/bin/FSharpD3.dll"


// http://chimera.labs.oreilly.com/books/1230000000345/ch11.html#_force_layout

open FSharpD3
open D3
open FunScript
open FunScript.TypeScript

open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection


let inline compile_js expression =
    Compiler.compileWithoutReturn expression


[<FunScript.JS>]
module Data =  

    type Node = 
        {   name  : string;
            size  : float
            color : string
            index : int;
            x     : float; 
            y     : float
        }

    type Edge = 
        {
            source : Node
            target : Node
            color : string
            size       : float 
        }

module Quote =
    
    open Data

    ///
    let nodeExpr (x:Node) =
        Expr.NewRecord(
            typeof<Node>,
            [
                Expr.Value x.name
                Expr.Value x.size
                Expr.Value x.color
                Expr.Value x.index
                Expr.Value x.x
                Expr.Value x.y
            ])

    ///
    let quoteNodeArr (series:Node []) =
        Expr.NewArray(
            typeof<Node>,
            [for x in series -> nodeExpr x])

    ///
    let edgeExpr (x:Edge) =
        Expr.NewRecord(
            typeof<Edge>,
            [
                nodeExpr x.source
                nodeExpr x.target            
                Expr.Value x.color
                Expr.Value x.size
            ])

    ///
    let quoteEdgeArr (series:Edge []) =
        Expr.NewArray(
            typeof<Edge>,
            [for x in series -> edgeExpr x])






[<ReflectedDefinition>]
module ChartArea =
    
    let createSvgContainer width height =
        d3.select "#container"
        |> append "svg"
        |> attrVal "width" width
        |> attrVal "height" height 



[<ReflectedDefinition>]
module Chart =
    open Data
    
    let forceGraph edgeList =

        let width = 640.
        let height = 480.
            
        let n = [|for i in edgeList do yield! [i.source;i.target;] |] |> Seq.distinctBy (fun n -> n.index) |> Seq.sortBy (fun n -> n.index)  |> Seq.toArray
                
        let e = 
            edgeList
            |> Array.map (fun e -> { e with source = n.[e.source.index];target = n.[e.target.index] } )

        let svg = ChartArea.createSvgContainer width height      

        let force = 
            force ()
            |> size width height 
            |> D3.Layout.nodes n
            |> D3.Layout.links e
            |> D3.Layout.linkDistanceF (fun (e) _ -> (e:?>Edge).size)
            |> charge -100.
            //|> linkDistance 200.
            //|> D3.Layout.linkStrength (fun (e) _ -> (e:?>Edge).size)
            //|> start

        let edges =  
            svg
            |> selectAll "line"
            |> data e
            |> enter
            |> append "line"
            |> attr "stroke" (fun e _ -> "grey")
            |> attr "stroke-width" (fun e _ -> 2.5)
//                |> attr "stroke" (fun e _ -> e.color)
//                |> attr "stroke-width" (fun e _ -> e.size / 100.)

        let nodes = 
            svg
            |> selectAll "circle"
            |> data n
            |> enter
            |> append "circle"
            |> attr "r" (fun n _ -> n.size)
            |> attr "fill" (fun n _ -> n.color)
//                |> call (fun _ -> force.drag() |> ignore)
            //|> style "fill" "gray"
//
        let labels =
            svg
            |> selectAll "text"
            |> data n
            |> enter
            |> append "text"
//            |> attrVal "font" "9px helvetica"
            |> attrVal "font-size" "20px"
            |> attr "dx" (fun n _ -> n.size + 5.)
            |> attrVal "dy" ".35em"
            |> text (fun d _ -> d.name)
            |> style "stroke" "gray"


        let tickF() = 
            nodes
            |> attr "cx" (fun (n:Node) _ -> n.x)
            |> attr "cy" (fun (n:Node) _ -> n.y)
            |> ignore

            edges
            |> attr "x1" (fun (e:Edge) _ -> e.source.x)
            |> attr "y1" (fun (e:Edge) _ -> e.source.y)
            |> attr "x2" (fun (e:Edge) _ -> e.target.x)
            |> attr "y2" (fun (e:Edge) _ -> e.target.y)
            |> ignore

            labels
            |> attr "x" (fun n _ -> n.x)
            |> attr "y" (fun n _ -> n.y)
            |> ignore

        do        
            force 
            |> start
            |> on "tick" tickF
            |> ignore




    let show (jsCode:string) =
        let wnd,browser = FSharpD3.ViewContainer.createContainerWithBrowser()
        browser.NavigateToString (HtmlScaffold.common jsCode)



let forceGraph config =
    let configExpr = Quote.quoteEdgeArr config
    compile_js <@ Chart.forceGraph %%configExpr @>



open Data

// ################### TEST
let nodesData = [|
        { name = "Adam"    ; color = "grey"; size = 10.; index = 0; x = 320. ; y = 240.};
        { name = "Bob"     ; color = "blue"; size = 20.; index = 1; x = 320. ; y = 240.};
        { name = "Carrie"  ; color = "grey"; size = 5.;  index = 2; x = 320. ; y = 240.};
        { name = "Donovan" ; color = "pink"; size = 15.; index = 3; x = 320. ; y = 240.};
        { name = "Edward"  ; color = "grey"; size = 10.; index = 4; x = 320. ; y = 240.};
        { name = "Felicity"; color = "blue"; size = 20.; index = 5; x = 320. ; y = 240.};
        { name = "George"  ; color = "grey"; size = 5.;  index = 6; x = 320. ; y = 240.};
        { name = "Hannah"  ; color = "pink"; size = 15.; index = 7; x = 320. ; y = 240.};
        { name = "Iris"    ; color = "grey"; size = 5.;  index = 8; x = 320. ; y = 240.};
        { name = "Jerry"   ; color = "pink"; size = 15.; index = 9; x = 320. ; y = 240.};
    |]



let edgesData = [|
        { source = nodesData.[0]; target = nodesData.[1]; color = "grey"; size = 85.;};
        { source = nodesData.[0]; target = nodesData.[2]; color = "green";size = 70.;};
        { source = nodesData.[0]; target = nodesData.[3]; color = "grey"; size = 70.;};
        { source = nodesData.[0]; target = nodesData.[4]; color = "grey"; size = 70.;};
        { source = nodesData.[1]; target = nodesData.[5]; color = "grey"; size = 85.;};
        { source = nodesData.[2]; target = nodesData.[5]; color = "green";size = 70.;};
        { source = nodesData.[2]; target = nodesData.[5]; color = "grey"; size = 70.;};
        { source = nodesData.[3]; target = nodesData.[4]; color = "grey"; size = 70.;};
        { source = nodesData.[5]; target = nodesData.[8]; color = "grey"; size = 85.;};
        { source = nodesData.[5]; target = nodesData.[9]; color = "green";size = 70.;};
        { source = nodesData.[6]; target = nodesData.[7]; color = "grey"; size = 70.;};
        { source = nodesData.[7]; target = nodesData.[8]; color = "grey"; size = 70.;};
        { source = nodesData.[8]; target = nodesData.[9]; color = "grey"; size = 70.;}
    |]



forceGraph edgesData |> Chart.show



