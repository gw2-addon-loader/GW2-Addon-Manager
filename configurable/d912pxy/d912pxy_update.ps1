#Matthew Lee
#Summer 2019
#d912pxy update script
#Not affiliated with megai2 or d912pxy - I wrote this for my own education and convenience purposes


##########PREFERENCES###########
#location script is run from
$rootRunPath = (Split-Path $PSScriptRoot -Parent)
#place to temporarily store the zipped download version before extracting into game directory
$updatedVersionPath = "$rootRunPath\d912pxy\d912pxy_latest.zip"
#config file
$configs = Get-Content "$rootRunPath\dll_config.ini" | ConvertFrom-Json
#Guild Wars 2 directory
$GuildWars2Path = $configs.game_path
#preferred name for the dll file in /Guild Wars 2/bin64 - e.g. d3d9.dll, d3d9_chainload.dll, etc. Can change in dll_configs.ini file.
$dll_name = $configs.d912pxy
################################

#url for latest release info on Github API
$url = "https://api.github.com/repos/megai2/d912pxy/releases/latest"
$release_dll_path = "\d912pxy\dll\release\d3d9.dll"

#regex and command to find current d912pxy version in log file
$versionRegex = 'v\d+\.\d+\.*\d*'
$currentVersion = select-string -Path "$GuildWars2Path\d912pxy\log.txt" -Pattern $versionRegex | %{$_.Matches} | %{$_.Value}

#avoid some TLS/SSL issues I ran into
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12



#get latest release info from Github API
$gitResponse = Invoke-RestMethod -Method Get -URI $url
$releaseNo = $gitResponse.tag_name
#address for downloading release
$downloadURL = $gitResponse.assets.browser_download_url

function update
{
	#downloading latest release
	wget $downloadURL -outfile $updatedVersionPath
	#unzipping downloaded release to Guild Wars 2
	Expand-Archive $updatedVersionPath -DestinationPath $GuildWars2Path -Force
    #copy d3d9
    Copy-Item "$GuildWars2Path$release_dll_path" -Destination "$GuildWars2Path\bin64\\$dll_name" -Force
    #update version number (since otherwise will not be updated until next run of d912pxy/GW2)
    (Get-Content "$GuildWars2Path\d912pxy\log.txt") -replace $versionRegex, $releaseNo | Set-Content "$GuildWars2Path\d912pxy\log.txt"
    #remove downloaded zip from download location
    Remove-Item $updatedVersionPath
}

if(-NOT ($releaseNo -eq $currentVersion))
{
	update
}

