param([string]$QuestionArgs="c:\dockerExchange\input.csv output.csv")

$env:Path="C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\;C:\Program Files\Docker\Docker\resources\bin\"

# Docker image name for the application
$ImageName="profesor79pl/profesor79_pl_actor_scaling_example_target"



try {
$Computer = "."  # For local hosts
$Flag=0
Get-WmiObject Win32_NetworkAdapterConfiguration -ComputerName  $Computer  | ForEach-Object { IF($_.IPEnabled -eq "$True" -and $_.DNSDomain.Length  -gt  1)  { 
    $DNS=$_.DNSServerSearchOrder ; $Domain = $_.DNSDomain ; $Flag = 1 } }
       If ($Flag -eq 1) {
			$arr = $DNS -split " "
			$a =$arr[0] 
			Write-OutPut "dns server $a"    
		       }   
Else{Write-Host  -ForegroundColor Red "DNS Servers is not found : IPEnabled is False !"  
     Write-Host "Error Message"  
     Exit 1001}
     }   
catch {
Write-Host "Error Message" 
Exit 1001
}



function Invoke-Docker-Run ([string]$DockerImage, [string]$Question) {
	echo "starting remote deploy target"
	Invoke-Expression "docker run --dns=$a  --cpus=`"2.0`" -v c:/dockerExchange:c:/dockerExchange --rm $ImageName "
}



Invoke-Docker-Run    -DockerImage $ImageName
