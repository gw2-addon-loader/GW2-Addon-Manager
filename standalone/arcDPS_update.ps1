#Matthew Lee
#Summer 2019
#arcDPS update script
#Not affiliated with Deltaconnected or arcDPS


### CONFIG/PREFS ###
#location script is run from
$rootRunPath = (Split-Path $PSScriptRoot -Parent)
#Guild Wars 2 directory
$GuildWars2Path = "C:\Program Files\Guild Wars 2"
#preferred name for the dll file in /Guild Wars 2/bin64 - e.g. d3d9.dll, d3d9_chainload.dll, etc.
$arc_name = "d3d9.dll"
#name of build templates dll - leave as is unless directions on arcDPS website indicate a different name
$buildtemplates_name = "d3d9_arcdps_buildtemplates.dll"
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

#download latest md5 hash from the url
wget $md5_hash_url -outfile $rootRunPath\d3d9.dll.md5sum

if(-NOT(Test-Path $md5_path))
{
    update
    echo "exists"
}
elseif(-NOT((Get-Content $md5_path) -eq (Get-Content $rootRunPath\d3d9.dll.md5sum)))
{
    update
    echo "not match"
}

Remove-Item $rootRunPath\d3d9.dll.md5sum

    
