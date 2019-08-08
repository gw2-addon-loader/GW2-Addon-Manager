#Matthew Lee
#Summer 2019
#update all add-ons

$arcDPS_update = "$PSScriptRoot\ArcDPS\arcDPS_update.ps1"
$gw2radial_update = "$PSScriptRoot\GW2Radial\gw2Radial_update.ps1"
$d912pxy_update = "$PSScriptRoot\d912pxy\d912pxy_update.ps1"

$config_file = "$PSScriptRoot\dll_config.ini"

Write-Host "Welcome to the GW2 Add-On Updater!"

$edit_config = Read-Host "Would you like to edit the configuration file? [Yes/No | Default: No]"
if($edit_config -eq "Yes")
{
    #Add an automatic dll file namer based on the add-ons the user says they have installed
    Write-Host "Entering config file edit mode. Press enter without typing on a prompt to skip it."
    $gamePath = Read-Host "Guild Wars 2 Game Path"
    $arc_dll = Read-Host "ArcDps dll filename"
    $arc_templates_dll = Read-Host "ArcDps Build Templates dll filename"
    $gw2radial_dll = Read-Host "GW2Radial dll filename"
    $d912pxy_dll = Read-Host "d912pxy dll filename"
    
    $config_json = Get-Content $config_file -raw | ConvertFrom-Json
    
    if($gamePath){$config_json.game_path = $gamePath}
    if($arc_dll){$config_json.arcDPS = $arc_dll}
    if($arc_templates_dll){$config_json.arcDPS_buildTemplates = $arc_templates_dll}
    if($gw2radial_dll){$config_json.gw2Radial = $gw2radial_dll}
    if($d912pxy_dll){$config_json.d912pxy = $d912pxy_dll}
    
    $config_json | ConvertTo-Json | Set-Content $config_file
}

Write-Host -NoNewLine "Checking for updates for arcDPS..."
& $arcDPS_update
Write-Host "done."

Write-Host -NoNewLine "Checking for updates for GW2Radial..."
& $gw2radial_update
Write-Host "done."

Write-Host -NoNewLine "Checking for updates for d912pxy..."
& $d912pxy_update
Write-Host "done."

Write-Host "All update checks and/or downloads completed, press enter to close this window."
$Host.UI.ReadLine()
exit