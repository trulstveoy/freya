﻿namespace System
open System.Reflection

[<assembly: AssemblyDescriptionAttribute("Freya")>]
[<assembly: AssemblyFileVersionAttribute("1.0.0")>]
[<assembly: AssemblyProductAttribute("Freya.Recorder")>]
[<assembly: AssemblyTitleAttribute("Freya.Recorder")>]
[<assembly: AssemblyVersionAttribute("1.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0.0"
