param([string]$QuestionArgs="c:\dockerExchange\input.csv output.csv")

$env:Path="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\;C:\Program Files\Docker\Docker\resources\bin\"

# Docker image name for the application
$ImageName="profesor79pl/profesor79_pl_actor_scaling_example_target"

function Invoke-Docker-Run ([string]$DockerImage, [string]$Question) {
	echo "starting remote deploy target"
	Invoke-Expression "docker run --cpus=`"2.0`" -v c:/dockerExchange:c:/dockerExchange --rm $ImageName "
}

Invoke-Docker-Run    -DockerImage $ImageName
