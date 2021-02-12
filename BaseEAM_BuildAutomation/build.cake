
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var solutionFile = GetFiles("D:/MillDol/BaseEAM_Cloud_WebApplication/BaseEAM/*.sln").First();
var solution = new Lazy<SolutionParserResult>(() => ParseSolution(solutionFile));

// Define directories.
var buildWebDir = Directory("D:/MillDol/BaseEAM_Cloud_WebApplication/BaseEAM/BaseEAM.Web/bin") + Directory(configuration);
var distWebDir = Directory("D:/MillDol/BaseEAM_Cloud_SolutionItems/Build/Web");

var buildWebApiDir = Directory("D:/MillDol/BaseEAM_Cloud_WebApplication/BaseEAM/BaseEAM.WebApi/bin") + Directory(configuration);
var distWebApiDir = Directory("D:/MillDol/BaseEAM_Cloud_SolutionItems/Build/WebApi");

var buildWorkflowDir = Directory("D:/MillDol/BaseEAM_Cloud_Workflow/BaseEAM.Workflow/BaseEAM.WorkflowService/bin") + Directory(configuration);
var distWorkflowDir = Directory("D:/MillDol/BaseEAM_Cloud_SolutionItems/Build/WorkflowService");

var buildBkgDir = Directory("D:/MillDol/BaseEAM_Cloud_BackgroundService/BaseEAM.BackgroundService/BaseEAM.BackgroundService/bin") + Directory(configuration);
var distBkgDir = Directory("D:/MillDol/BaseEAM_Cloud_SolutionItems/Build/BackgroundService");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildWebDir);
	CleanDirectory(buildWebApiDir);
	CleanDirectory(buildWorkflowDir);
	CleanDirectory(buildBkgDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("D:/MillDol/BaseEAM_Cloud_WebApplication/BaseEAM/BaseEAM.sln");
	NuGetRestore("D:/MillDol/BaseEAM_Cloud_Workflow/BaseEAM.Workflow/BaseEAM.Workflow.sln");
	NuGetRestore("D:/MillDol/BaseEAM_Cloud_BackgroundService/BaseEAM.BackgroundService/BaseEAM.BackgroundService.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("D:/MillDol/BaseEAM_Cloud_WebApplication/BaseEAM/BaseEAM.sln", settings =>
        settings.SetConfiguration(configuration));
		
		// Use MSBuild
      MSBuild("D:/MillDol/BaseEAM_Cloud_Workflow/BaseEAM.Workflow/BaseEAM.Workflow.sln", settings =>
        settings.SetConfiguration(configuration));
		
		// Use MSBuild
      MSBuild("D:/MillDol/BaseEAM_Cloud_BackgroundService/BaseEAM.BackgroundService/BaseEAM.BackgroundService.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
    }
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
{
	var webProject = solution.Value
			.Projects
			.Where(p => p.Name.EndsWith(".Web"))
			.First();
			
	DotNetBuild(webProject.Path, settings => settings
		.SetConfiguration(configuration)
		.WithProperty("DeployOnBuild", "true")
		.WithProperty("WebPublishMethod", "FileSystem")
		.WithProperty("DeployTarget", "WebPublish")
		.WithProperty("publishUrl", distWebDir)
		.SetVerbosity(Verbosity.Minimal));
		
	var webApiProject = solution.Value
		.Projects
		.Where(p => p.Name.EndsWith(".WebApi"))
		.First();
			
	DotNetBuild(webApiProject.Path, settings => settings
		.SetConfiguration(configuration)
		.WithProperty("DeployOnBuild", "true")
		.WithProperty("WebPublishMethod", "FileSystem")
		.WithProperty("DeployTarget", "WebPublish")
		.WithProperty("publishUrl", distWebApiDir)
		.SetVerbosity(Verbosity.Minimal));
		
	CopyDirectory(buildWorkflowDir, distWorkflowDir);
	
	CopyDirectory(buildBkgDir, distBkgDir);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
	.IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
