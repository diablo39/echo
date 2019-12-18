#load ".fake/build.fsx/intellisense.fsx"

// Clean ->  Restore all -> Build app -> build & run unit tests -> build & run integration tests -> publish -> docker build -> docker publish
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.DotNet.Testing.XUnit2


Shell.cd("..")

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    |> Shell.cleanDirs 
)

Target.create "Restore" (fun _ ->
    !! "*.sln"
    |> Seq.iter (DotNet.restore (fun args -> 
    { args with 
        Sources = [
            "https://api.nuget.org/v3/index.json"
        ]
    }) )
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    ++ "tests/**/*.*proj"
    |> Seq.iter (DotNet.build (fun p -> 
    { p with   
        NoRestore = true
    }))
)

Target.create "Run tests" (fun _ ->
   !! "tests/**/*.*proj"
   |> Seq.iter (DotNet.test (fun p -> 
   { p with   
       NoRestore = true
       NoBuild = true 
       Logger = Some "xunit;LogFilePath=test_result.xml"
   }))
)

// Target.create "Run tests" (fun _ ->
//      !! ("tests/**/bin/**/*.UnitTests.dll")
//      |> Fake.DotNet.Testing.XUnit2.run (fun p -> 
//      { p with 
//         NUnitXmlOutputPath = Some ("xml.xml")
//      })
// )


Target.create "All" ignore


"Clean"
    ==> "Restore"
    ==> "Build" 
    ==> "Run tests"
//     ==> "UnitTests" <=> "IntegrationsTests"
//         ==> "Docker build"
//         ==> "Docker publish"
         ==> "All"

Target.runOrDefault "All"
