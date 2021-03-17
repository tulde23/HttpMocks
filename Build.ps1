<#
.Synopsis
	Builds a the Practices.Testing Solution

.Description
	Builds the Practices.Testing solution, runs unit and integration tests, performs code coverage analysis and generates a report.

.Example
	Run-Build -projectName "web-core" -incrementalBuildNumber 10
#>
[CmdletBinding(SupportsShouldProcess = $true)]
param(

	[Parameter(HelpMessage = "The  current build configuration (Debug|Releae)")]
	[ValidateSet('Debug','Release')]
	[string] $configuration='Release',
    [Parameter(HelpMessage = "The current build number from CI")]
    [int] $incrementalBuildNumber,
	[Parameter(HelpMessage = "Runs unit tests with coverage")]
    [switch] $unitTests,
	[Parameter(HelpMessage = "Runs integration tests with coverage")]
    [switch] $integrationTests,
	[Parameter(HelpMessage ="Disable Code Coverage")]
	[switch] $disbleCodeCoverage,
	[Parameter(HelpMessage="Disable Publish")]
	[switch] $disablePublish,
	[Parameter(HelpMessage="Enables Package Restore")]
	[switch] $enablePackageRestore,
    [Parameter(HelpMessage="Enables Local Consul Configuration")]
	[switch] $disableJenkinsConfig,
    [Parameter(HelpMessage="Flag indicating if the profiler should run as admin")]
	[switch] $isUser,
    [Parameter(HelpMessage="CodeCoverage Nuget Package Version")]
    [string] $toolsVersion = "3.0.3"


)
#reset error state
$global:LASTEXITCODE = 0
<#
    .Synopsis
        A common logger method
#>
function logger() {
    param
    (
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [string]$msg,
        [string]$BackgroundColor = "DarkBlue",
        [string]$ForegroundColor = "Magenta"
    )

    Write-Host -ForegroundColor $ForegroundColor $msg;
}


<#
    .Synopsis
        Invokes "dotnet publish" on the API
#>
function publish($disablePublish, $publishProjectName, $configuration, $version){
if ( $disablePublish -eq $False -and $LASTEXITCODE -eq 0){
	logger "dotnet publish $PSScriptRoot\src\$publishProjectName"
	dotnet publish "$PSScriptRoot\src\$publishProjectName" -c $configuration -o "site" -r win10-x64 --version-suffix $version
}

}


<#
    .Synopsis
        Copys unit/integration test results up to the report folder
#>
function copyTestResults($projectName, $name){

      $path =  "$PSScriptRoot\tests\$projectName\TestResults\TestResults.$name"
      if( (Test-Path $path ) -eq $True){
        Copy-Item $path "$reportFolder\TestResults.$name"
      }
}

logger "##################################"
logger "# Building Practices.Testing   #"
logger "##################################"
 
 $logo = "
                     .::.
                  .:'  .:
        ,MMM8&&&.:'   .:'
       MMMMM88&&&&  .:'
      MMMMM88&&&&&&:'
      MMMMM88&&&&&&
    .:MMMMM88&&&&&&
  .:'  MMMMM88&&&&
.:'   .:'MMM8&&&'
:'  .:'
'::'  
 ";
 $versionMajor = (Get-Date).Year
$versionMinor = (Get-Date).Month
$versionBuild = 0# (Get-ChildItem env:BUILD_NUMBER).Value
$versionString = "$versionMajor.$versionMinor.$versionBuild"


logger $logo

logger "Build Configuration: $configuration"
logger "Build Number: $incrementalBuildNumber"
logger "Code Coverage: $disbleCodeCoverage"
logger "Publishing: $disablePublish"
logger "Version: $versionString"
 

$reportFolder = "$PSScriptRoot\reports"
$unitTestProject           = "UnitTests"
$publishProjectName     = "Practices.Testing"
$coverageReport = "$PSScriptRoot\tests\$unitTestProject\reports\coverage.cobertura.xml"

 dotnet --info

if( Test-Path $reportFolder)
{
	Remove-Item $reportFolder -Force -Recurse
}
$dir = New-Item -Path $reportFolder -type Directory


#run unit tests
if( $disbleCodeCoverage){
    logger "Code coverage is disabled..."
	dotnet test ".\tests\$unitTestProject\$unitTestProject.csproj"  "--logger:trx;LogFileName=TestResults.Unit_Tests.xml"
	 publish -disablePublish $disablePublish -publishProjectName $publishProjectName -configuration $configuration -version $versionString
	
}
else{
	dotnet test ".\tests\$unitTestProject\$unitTestProject.csproj" /p:CollectCoverage=true /p:CoverletOutput='./reports/' /p:CoverletOutputFormat=cobertura /p:Exclude="[xunit*]*%2c*"  "--logger:trx;LogFileName=TestResults.Unit_Tests.xml"
	If (Test-Path $coverageReport){
	 logger "Copying $coverageReport to $reportFolder\cobertura.xml"
     Copy-Item -Path  $coverageReport -Destination "$reportFolder\cobertura.xml" -Force
     #Remove-Item  $coverageReport
	 logger "Copied cobertura report $coverageReport"
	 copyTestResults -projectName $unitTestProject -name "Unit_Tests.xml"
	 logger "Copied test results."
      publish -disablePublish $disablePublish -publishProjectName $publishProjectName -configuration $configuration -version $versionString
	  logger "Build completed...."
 }
}






