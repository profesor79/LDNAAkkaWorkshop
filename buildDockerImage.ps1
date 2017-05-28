<#
.SYNOPSIS
   Build the Console Application and Docker Image for Windows container
.DESCRIPTION
    Build the Console Application and Docker Image for Windows container
#>

Set-StrictMode -Version Latest
$ErrorActionPreference="Stop"
$ProgressPreference="SilentlyContinue"

# Docker image name for the application
$ImageName="profesor79_pl_actor_scaling_example"

function Invoke-MSBuild ([string]$MSBuildPath, [string]$MSBuildParameters) {
    Invoke-Expression "$MSBuildPath $MSBuildParameters"
}

function Invoke-Docker-Build ([string]$ImageName, [string]$ImagePath, [string]$DockerBuildArgs = "") {
    echo "docker build -t $ImageName $ImagePath $DockerBuildArgs"
    Invoke-Expression "docker build -t $ImageName $ImagePath $DockerBuildArgs"
}

$env:Path="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\;C:\Program Files\Docker\Docker\resources\bin\"

$msbuil="MSBuild.exe"
Invoke-MSBuild -MSBuildPath $msbuil -MSBuildParameters ".\Profesol79.Merge.Consoler\Profesor79.Merge.Consoler.csproj /p:OutputPath=.\publish /p:Configuration=Release"
Invoke-Docker-Build -ImageName $ImageName -ImagePath ".\Profesol79.Merge.Consoler\."
