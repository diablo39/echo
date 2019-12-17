#r "paket:nuget Fake.DotNet.Cli"
#r "paket:nuget Fake.IO.FileSystem"
#r "paket:nuget Fake.DotNet.Testing.XUnit2"
#r "paket:nuget Fake.Core.Target"

#load ".fake/build.fsx/intellisense.fsx"

// Clean ->  Restore all -> Build app -> build & run unit tests -> build & run integration tests -> publish -> docker build -> docker publish
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.DotNet.Testing.XUnit2

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
    |> Seq.iter (DotNet.build id)
)

Target.create "Run tests" (fun _ ->

    !! ("src/**/*.*proj" @@ "xUnit.Test.*.dll")
    |> xUnit2 (fun p -> { p with HtmlOutputPath = Some (testDir @@ "xunit.html") })

    
)


Target.create "All" ignore


"Clean"
    ==> "Restore"
    ==> "Build" 
    ==> "Run integration test"
//     ==> "UnitTests" <=> "IntegrationsTests"
//         ==> "Docker build"
//         ==> "Docker publish"
         ==> "All"

Target.runOrDefault "All"
