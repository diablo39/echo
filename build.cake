var target = Argument("target", "All");
var isTeamCityBuild = TeamCity.IsRunningOnTeamCity;


Task("Clean")
  .Does(() => { })
  .DoesForEach(GetFiles("*.sln"), (file) => 
  { 
      DotNetCoreClean(file.FullPath);
  });

Task("Restore")
  .Does(() => { })
  .IsDependentOn("Clean")
  .DoesForEach(GetFiles("*.sln"), (file) => 
  { 
      DotNetCoreRestore(file.FullPath);
  });

Task("Build")
  .Does(() => { })
  .IsDependentOn("Restore")
  .DoesForEach(GetFiles("*.sln"), (file) => 
  { 
      var buildSetting = new DotNetCoreBuildSettings { 
        NoRestore = true 
      };

      DotNetCoreBuild(file.FullPath, buildSetting);
  });

Task("Run rests")
  .Does(() =>{})
  .IsDependentOn("Build")
  .DoesForEach(GetFiles("tests/**/*.*proj"), (file) => {
    
    var testSetting = new DotNetCoreTestSettings { 
        NoRestore = true,
        Logger = "console;verbosity=normal" 
    };
    
    DotNetCoreTest(file.FullPath, testSetting);

  });

Task("All")
  .Does(() =>{ })
  .IsDependentOn("Run rests");
 


RunTarget(target);