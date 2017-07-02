# Powershel script will give the user a list of the DNS servers a local or remote computer , and # #DNS Domain .
$ErrorActionPreference = "Stop"
try {
$Computer = "."  # For local hosts
$Flag=0
Get-WmiObject Win32_NetworkAdapterConfiguration -ComputerName  $Computer  | ForEach-Object { IF($_.IPEnabled -eq "$True" -and $_.DNSDomain.Length  -gt  1)  { 
    $DNS=$_.DNSServerSearchOrder ; $Domain = $_.DNSDomain ; $Flag = 1 } }
       If ($Flag -eq 1) { Write-OutPut "DNS Servers Is    $DNS "DNS" Domain is $Domain"  
$arr = $DNS -split " "
$a =$arr[0] 
Write-OutPut "aaa $a"  
     Write-Host ("Script Check Passed")
       Exit 0
       }   
Else{Write-Host  -ForegroundColor Red "DNS Servers is not found : IPEnabled is False !"  
     Write-Host "Error Message"  
     Exit 1001}
     }   
catch {
Write-Host "Error Message" 
Exit 1001
}