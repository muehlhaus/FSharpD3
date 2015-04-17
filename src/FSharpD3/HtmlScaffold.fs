namespace FSharpD3

module HtmlScaffold =

    /// Generates the HTML document for common scaffold
    let common js =
        sprintf """<!DOCTYPE html>
                    <html>
                        <head>
                            <meta http-equiv="X-UA-Compatible" content="IE=9" >
                            <title>FsJVis</title>                                                  
                            <script src="http://code.jquery.com/jquery-1.8.0.js"></script>
                            <script type="text/javascript" src="http://d3js.org/d3.v3.min.js"></script>
                        </head>
                        <body>

                            <div id="container">          
                            </div>

                            <script>
                    %s
                            </script>
                        </body>
                    </html>"""          js


    