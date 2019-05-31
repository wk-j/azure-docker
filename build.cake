#addin "wk.StartProcess"
#addin "wk.ProjectParser"

using PS = StartProcess.Processor;
using ProjectParser;

var name = "MyApi";

var currentDir = new DirectoryInfo(".").FullName;
var info = Parser.Parse($"src/{name}/{name}.csproj");
var publishDir = ".publish";
var version = DateTime.Now.ToString("yy.MM.dd.HHmm");

Task("Publish").Does(() => {
    CleanDirectory(publishDir);

    var settings = new DotNetCoreMSBuildSettings();
    settings.Properties["Version"] = new string[] { version };

    DotNetCorePublish($"src/{name}", new DotNetCorePublishSettings {
        OutputDirectory = $"{publishDir}/W",
        MSBuildSettings = settings,
        Runtime = "linux-x64"
        // Runtime = "linux-musl-x64"
    });
});

Task("Pack").Does(() => {
    var settings = new DotNetCoreMSBuildSettings();
    settings.Properties["Version"] = new string[] { version };

    CleanDirectory(publishDir);
    DotNetCorePack($"src/{name}", new DotNetCorePackSettings {
        OutputDirectory = publishDir,
        MSBuildSettings = settings
    });
});

Task("Build-Docker")
    .IsDependentOn("Publish")
    .Does(() => {
        PS.StartProcess($"docker build -t azure-docker:latest .");
        PS.StartProcess($"docker tag  azure-docker:latest repo.treescale.com/wk/azure-docker:{version}");
        PS.StartProcess($"docker tag  azure-docker:latest repo.treescale.com/wk/azure-docker:latest");
        PS.StartProcess($"docker push repo.treescale.com/wk/azure-docker:{version}");
        PS.StartProcess($"docker push repo.treescale.com/wk/azure-docker:latest");
    });

var target = Argument("target", "Pack");
RunTarget(target);
