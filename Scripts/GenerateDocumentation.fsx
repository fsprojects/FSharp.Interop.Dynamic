#!/bin/sh
#if bin_sh
  # Doing this because arguments can't be used with /usr/bin/env on linux, just mac
  exec fsharpi --define:mono_posix --exec $0 $*
#endif

#if mono_posix
#r "Mono.Posix.dll"
open Mono.Unix.Native
let applyExecutionPermissionUnix path =
    let _,stat = Syscall.lstat(path)
    Syscall.chmod(path, FilePermissions.S_IXUSR ||| stat.st_mode) |> ignore
#else
let applyExecutionPermissionUnix _ = ()
#endif

open System
open System.IO
open System.Diagnostics

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let createDir d= Directory.CreateDirectory(d) |> ignore

let paketPath = Path.Combine("bin", "paket.exe")
if not (File.Exists paketPath) then
    let url = "http://fsprojects.github.io/Paket/stable"
    use wc = new Net.WebClient()
    let tmp = Path.GetTempFileName()
    let stable = wc.DownloadString(url)
    wc.DownloadFile(stable, tmp)
    createDir "bin"
    File.Move(tmp, paketPath)
let exec (exePath:string) (args:string seq) =
    let processStart (psi:ProcessStartInfo) =
        let ps = Process.Start(psi)
        ps.WaitForExit ()
        ps.ExitCode
    applyExecutionPermissionUnix exePath
    let exitCode = ProcessStartInfo(
                        exePath,
                        args |> String.concat " ",
                        UseShellExecute = false) 
                   |> processStart
    if exitCode <> 0 then
        exit exitCode
    ()

exec paketPath ["install"]
printfn "Finished Paket install."

#I "packages/FSharp.Compiler.Service/lib/net45/"
#r "FSharp.Compiler.Service.dll"
#I "packages/FSharp.Formatting/lib/net40/"
#r "RazorEngine.dll"
#r "FSharp.MetadataFormat.dll"
#r "FSharp.Literate.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.Formatting.Razor.dll"
#I "packages/FSharp.Data/lib/net45/"
#r "FSharp.Data.dll"
#r "System.Xml.Linq.dll"
open FSharp.Formatting.Razor
open FSharp.Data

/// variables
type FsProj = XmlProvider<"../FSharp.Interop.Dynamic/FSharp.Interop.Dynamic.fsproj">
let fsProj = FsProj.GetSample()

let targetFramework = fsProj.PropertyGroup.TargetFrameworks.Split(';') 
                        |> Seq.find (fun x-> x.Contains("net4"))
let projName = "FSharp.Interop.Dynamic"
let configuration = "Release"
let root = Path.Combine(__SOURCE_DIRECTORY__, "..")
let srcDir =  Path.Combine(root)
let testDir = Path.Combine(root, "Tests")
let docContent = Path.Combine(root, "DocsSrc")
let outputDir = Path.Combine(root, "docs")

let getDllNamed name =
    Path.Combine(srcDir,
                        projName,
                        "bin",
                        configuration,
                        targetFramework, 
                        sprintf "%s.dll" name)

let dll = getDllNamed projName


                        

///end variables



printfn "Copy Doc Content."

createDir outputDir

let docStyles = Path.Combine(docContent, "styles")
let baseOutput = Path.Combine(outputDir, "content")
createDir docStyles
for dir in Directory.GetDirectories(docStyles) do
    let outputContentDir = Path.Combine(baseOutput, Path.GetFileName(dir))
    createDir outputContentDir
    for file in Directory.GetFiles(dir) do
      File.Copy(file, Path.Combine(outputContentDir, Path.GetFileName(file)), overwrite = true)
for file in Directory.GetFiles(docStyles) do
    File.Copy(file, Path.Combine(baseOutput, Path.GetFileName(file)), overwrite = true)
let imgDir = Path.Combine(outputDir,"images")
createDir imgDir
let logo = Path.Combine(docContent, "logo.png")
File.Copy(logo, Path.Combine(imgDir, "logo.png"), overwrite=true)


let template = "docpage.cshtml"
let templateDirs = [ Path.Combine(docContent, "templates");
        Path.Combine(docContent, "templates", "reference") ]

let projInfo =
    [ "project-author",  fsProj.PropertyGroup.Authors
      "project-summary", fsProj.PropertyGroup.Description
      "project-github",  fsProj.PropertyGroup.PackageProjectUrl
      "project-nuget", sprintf "https://www.nuget.org/packages/%s" projName
      "project-name", projName
      "root", sprintf "/%s" projName
      ]


let options = sprintf "--reference:\"%s\"" dll

let processMdFile input output =
    RazorLiterate.ProcessMarkdown(
          Path.Combine(root,input),
          templateFile = template,
          output = Path.Combine(outputDir, output),
          replacements = projInfo,
          compilerOptions = options,
          layoutRoots = templateDirs,
          includeSource = true)
let processFsFile input output =
    RazorLiterate.ProcessScriptFile(
          input,
          templateFile = template,
          output = Path.Combine(outputDir, output),
          replacements = projInfo,
          compilerOptions = options,
          layoutRoots = templateDirs,
          includeSource = true )

//custom files
printfn "Generate Readme."

processMdFile (Path.Combine(root,"README.md")) "index.html"

printfn "Generate Other Docs."

processFsFile (Path.Combine(testDir,"Library1.fs")) "examples.html"

//end custom files
let refDir = Path.Combine(outputDir, "reference")
printfn "Generate API Reference. '%s'" dll
createDir(refDir)
let sourceRepo = sprintf "%s/tree/master/" fsProj.PropertyGroup.PackageProjectUrl
RazorMetadataFormat.Generate( dll, 
                              refDir,
                              templateDirs,
                              parameters = projInfo,
                              sourceRepo = sourceRepo,
                              sourceFolder = root)

printfn "Finished Generating Docs."