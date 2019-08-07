#Matthew Lee
#Summer 2019
#arcDPS update script
#Not affiliated with Deltaconnected or arcDPS


### CONFIG/PREFS ###
#Guild Wars 2 directory
$GuildWars2Path = "C:\Program Files\Guild Wars 2"
#config file
$configs = Get-Content "$GuildWars2Path\updater_scripts\dll_config.ini" | ConvertFrom-Json
#preferred name for the dll file in /Guild Wars 2/bin64 - e.g. d3d9.dll, d3d9_chainload.dll, etc. Can change in dll_configs.ini file.
$arc_name = $configs.arcDPS
#name of build templates dll - leave as is unless directions on arcDPS website indicate a different name
$buildtemplates_name = $configs.arcDPS_buildTemplates
#path to store md5
$md5_path = "$GuildWars2Path\d3d9.dll.md5sum"
#GW2 bin64 folder
$bin64 = "$GuildWars2Path\bin64"
####     ###     ####


### URLs ###
$arc_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll"
$buildtemplates_url = "https://www.deltaconnected.com/arcdps/x64/buildtemplates/d3d9_arcdps_buildtemplates.dll"
$md5_hash_url = "https://www.deltaconnected.com/arcdps/x64/d3d9.dll.md5sum"
###      ###

function update
{
    wget $arc_url -outfile $bin64\\$arc_name
    wget $buildtemplates_url -outfile $bin64\\$buildtemplates_name
    wget $md5_hash_url -outfile $md5_path
}


#avoid some TLS/SSL issues I ran into when testing the d912pxy update script
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

if(-NOT(Test-Path $md5_path))
{
    update
}
elseif(-NOT($md5_path -eq (wget $md5_hash_url -outfile $env:temp:)))
{
    update
}


    
