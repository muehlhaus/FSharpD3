namespace FSharpD3

open FunScript.TypeScript
open D3

    

[<FunScript.JS>]
module Chart =


    let fillBlue (selection: D3.Selection) =
        selection
        |> style "fill" "aliceblue" |> ignore

    
    let render (r:int) =
        let sampleSVG =
            d3.select "#container"
            |> append "svg"
            |> attrVal "width" 100
            |> attrVal "height" 100
        sampleSVG
        |> append "circle"
        |> style "stroke" "gray"
        |> style "fill" "white"
        |> attrVal "r" r
        |> attrVal "cx" 50
        |> attrVal "cy" 50
        |> call fillBlue
        
        

    