param([string]$QuestionArgs="input.csv output.csv")

$env:Path="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\;C:\Program Files\Docker\Docker\resources\bin\"

# Docker image name for the application
$ImageName="profesor79_pl_acotor_scaling_example"

function Invoke-Docker-Run ([string]$DockerImage, [string]$Question) {
	echo "Asking $Question"
	Invoke-Expression "docker run --rm $ImageName $Question"
}

Invoke-Docker-Run  $QuestionArgs --cpus=0.25  -DockerImage $ImageName
