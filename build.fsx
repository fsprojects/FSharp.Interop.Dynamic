#!/bin/sh
#if bin_sh
  # Doing this because arguments can't be used with /usr/bin/env on linux, just mac
  exec fsharpi --define:mono_posix --exec $0 $*
#endif
#if FSharp_MakeFile

(*
 * Single File Crossplatform FSharp Makefile Bootstrapper
 * Apache licensed - Copyright 2014 Jay Tuley <jay+code@tuley.name>
 * v 2.0 https://gist.github.com/jbtule/11181987
 *
 * How to use:
 *  On Windows `fsi --exec build.fsx <buildtarget>
 *    *Note:* if you have trouble first run "%vs120comntools%\vsvars32.bat" or use the "Developer Command Prompt for VS201X"
 *                                                           or install https://github.com/Iristyle/Posh-VsVars#posh-vsvars
 *
 *  On Mac Or Linux `./build.fsx <buildtarget>`
 *    *Note:* But if you have trouble then use `sh build.fsx <buildtarget>`
 *
 *)

#I "packages/FAKE/tools"
#r "FakeLib.dll"

open Fake
open Fake.DotNet.Testing.NUnit3
open Fake.DotNet.MsBuild

let sln = "./FSharp.Interop.Dynamic.sln"

let commonBuild target =
    let buildMode = getBuildParamOrDefault "buildMode" "Release"
    let setParams defaults =
            { defaults with
                ToolsVersion = Some("15.0")
                Verbosity = Some(Quiet)
                Targets = [target]
                Properties =
                    [
                        "Optimize", "True"
                        "DebugSymbols", "True"
                        "Configuration", buildMode
                    ]
             }
    build setParams sln |> DoNothing

Target "Restore" (fun () ->
    trace " --- Restore Packages --- "
    sln |> RestoreMSSolutionPackages (fun p ->
         { p with
             Retries = 4 })
)

Target "Clean" (fun () ->
    trace " --- Cleaning stuff --- "
    commonBuild "Clean"
)

Target "Build" (fun () ->
    trace " --- Building the libs --- "
    commonBuild "Build"
)

Target "Test" (fun () ->
    trace " --- Test the libs --- "
    let sendToAppveyer outFile = 
        let appveyor = environVarOrNone "APPVEYOR_JOB_ID"
        match appveyor with
            | Some(jobid) -> 
                use webClient = new System.Net.WebClient()
                webClient.UploadFile(sprintf "https://ci.appveyor.com/api/testresults/nunit/%s" jobid, outFile) |> ignore
            | None -> ()

    let testDirFromMoniker moniker = sprintf "./Tests/bin/Release/%s/" moniker
    let outputFileFromMoniker moniker = (testDirFromMoniker moniker) + (sprintf "TestResults.%s.xml" moniker)

    let testDir = testDirFromMoniker "net45"
    let outputFile = outputFileFromMoniker "net45"

    !! (testDir + "Tests.dll")
                       |> NUnit3 (fun p ->
                                 { p with
                                       Labels = All
                                       ResultSpecs = [outputFile] })
                    
    sendToAppveyer outputFile

    DotNetCli.Test
        (fun p -> 
             { p with 
                   Framework = "netcoreapp1.1"
                   Project = "Tests/Tests.fsproj"
                   Configuration = "Release"
                    })
    
)

"Restore"
  ==> "Build"
  ==> "Test"

RunTargetOrDefault "Test"


#else

open System
open System.IO
open System.Diagnostics

(* helper functions *)
#if mono_posix
#r "Mono.Posix.dll"
open Mono.Unix.Native
let applyExecutionPermissionUnix path =
    let _,stat = Syscall.lstat(path)
    Syscall.chmod(path, FilePermissions.S_IXUSR ||| stat.st_mode) |> ignore
#else
let applyExecutionPermissionUnix path = ()
#endif

let doesNotExist path =
    path |> Path.GetFullPath |> File.Exists |> not

let execAt (workingDir:string) (exePath:string) (args:string seq) =
    let processStart (psi:ProcessStartInfo) =
        let ps = Process.Start(psi)
        ps.WaitForExit ()
        ps.ExitCode
    let fullExePath = exePath |> Path.GetFullPath
    applyExecutionPermissionUnix fullExePath
    let exitCode = ProcessStartInfo(
                        fullExePath,
                        args |> String.concat " ",
                        WorkingDirectory = (workingDir |> Path.GetFullPath),
                        UseShellExecute = false)
                   |> processStart
    if exitCode <> 0 then
        exit exitCode
    ()

let exec = execAt Environment.CurrentDirectory

let downloadNugetTo path =
    let fullPath = path |> Path.GetFullPath;
    if doesNotExist fullPath then
        printf "Downloading NuGet..."
        use webClient = new System.Net.WebClient()
        fullPath |> Path.GetDirectoryName |> Directory.CreateDirectory |> ignore
        webClient.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", path |> Path.GetFullPath)
        printfn "Done."

let passedArgs = fsi.CommandLineArgs.[1..] |> Array.toList

(* execution script customize below *)

let makeFsx = fsi.CommandLineArgs.[0]

let nugetExe = ".nuget/NuGet.exe"
let fakeExe = "packages/FAKE/tools/FAKE.exe"

downloadNugetTo nugetExe

if doesNotExist fakeExe then
    exec nugetExe ["install"; "fake";  "-OutputDirectory packages"; "-ExcludeVersion"; "-PreRelease"]

exec fakeExe ([makeFsx; "-d:FSharp_MakeFile"] @ passedArgs)

#endif
