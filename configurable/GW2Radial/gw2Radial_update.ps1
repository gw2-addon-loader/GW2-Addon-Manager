#Matthew Lee
#Summer 2019
#GW2Radial update script
#Not affiliated with Friendly0Fire


### CONFIG/PREFS ###
#Guild Wars 2 directory
$GuildWars2Path = "C:\Program Files\Guild Wars 2"
#config file
$configs = Get-Content "$GuildWars2Path\updater_scripts\dll_config.ini" | ConvertFrom-Json
#preferred name for the dll file in /Guild Wars 2/bin64 - e.g. d3d9.dll, d3d9_chainload.dll, etc. Can change in dll_configs.ini file.
$dll_name = $configs.gw2Radial
####################

###### PATHS ######
#path to store the version number
$version_path = "$GuildWars2Path\updater_scripts\GW2Radial\version.txt"
#GW2 bin64 folder
$bin64 = "$GuildWars2Path\bin64"
#locations to download and unzip gw2radial before copying the dll into bin64
$zip_path = "$env:temp\gw2radial.zip"
$expanded_path = "$env:temp\gw2radial"
######         ######

### URLs ###
#url for latest release info on Github API
$git_url = "https://api.github.com/repos/Friendly0Fire/Gw2Radial/releases/latest"
###      ###


#TLS/SSL settings
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12


### GET LATEST RELEASE NUMBER ###
#get latest release info from Github API
$gitResponse = Invoke-RestMethod -Method Get -URI $git_url
$releaseNo = $gitResponse.tag_name
#address for downloading release
$downloadURL = $gitResponse.assets.browser_download_url
#######                   #######


### UPDATER ###
function update
{
    wget $downloadURL -outfile $zip_path
    Expand-Archive $zip_path -Destination $expanded_path -Force
    Copy-Item "$expanded_path\d3d9.dll" -Destination "$bin64\\$dll_name" -Force
    Set-Content -Path $version_path -Value $releaseNo
}
###         ###


#create version number file if it doesn't exist
if(-NOT(Test-Path $version_path))
{
    New-Item -Path $version_path -ItemType File -Value "" -Force
}

#current version of GW2Radial
$currentVersion = Get-Content $version_path

if(-NOT($currentVersion -eq $releaseNo))
{
    update
}

