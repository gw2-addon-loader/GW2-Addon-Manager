# GW2-UOAOM [![Current Version](https://img.shields.io/github/release/fmmmlee/GW2-Addon-Manager)](https://github.com/fmmmlee/GW2-Addon-Manager/releases) [![Github All Releases](https://img.shields.io/github/downloads/fmmmlee/GW2-Addon-Manager/total.svg)]() 
<a href="https://ci.appveyor.com/project/fmmmlee/gw2-addon-manager"><img src="https://ci.appveyor.com/api/projects/status/github/fmmmlee/gw2-addon-manager" alt="UI" align="right"/></a>


##### Guild Wars 2 Unofficial Add-On Manager
A tool to install and update some Guild Wars 2 add-ons without having to manually visit each website, check version numbers, download and rename dlls, etc.

### Current Status

Update 1.0.0 is nearing completion and will be releasing soon. A pre-release alpha **with no ArcDPS compatibility** is available on the [releases](https://github.com/fmmmlee/GW2-Addon-Manager/releases) page. Features include changing the configuration file to .yaml format, displaying addon descriptions and other information in a selection window, bug fixes, UI improvements, and a complete restructuring of the addon installation process to include and be compatible with the [addon loader](https://github.com/gw2-addon-loader).

&nbsp;

Current UI:

<p align="center">
<img src="https://user-images.githubusercontent.com/30479162/64070406-85a2e180-cc13-11e9-97ab-8911375cc15c.JPG" alt="v0.4.2 UI" width="450"/>
</p>

1.0.0 Alpha UI:

<p align="center">
<img src="https://user-images.githubusercontent.com/30479162/67152144-b6071380-f285-11e9-9a49-a6d3539d456d.JPG" alt="v1.0.0-al UI" width="450"/>
</p>

## Getting Started

Make sure your system meets the **Requirements** and that you don't have any unsupported add-ons (I hope to eventually expand the list). Don't run the program while GW2 or the GW2 launcher is running, as the program needs to access files that are locked while the game is active.

- Download the [latest release](https://github.com/fmmmlee/GW2-Addon-Manager/releases)
- Extract the zip file to any location you like
- Run "GW2 Addon Manager.exe" (see below for info about the first run)

#### First Time Setup
Make sure that the game path is set correctly (the default is C:\Program Files\Guild Wars 2). The first time GW2-UOAOM is run, **all selected add-ons will be redownloaded**. This is due to the way the application keeps track of what add-ons are installed and the version of each. Subsequent runs should only download new files if your version of an add-on is not the same as their latest release.

### Application Requirements:
- **Windows**
- **.NET Framework** (included in Windows 10)

### Currently Supported Add-Ons:
- <a href="https://www.deltaconnected.com/arcdps/">**arcDPS**</a>
- <a href="https://github.com/blish-hud/arcdps-bhud">**arcDPS Bhud Integration**</a>
- <a href="http://martionlabs.com/arcdps-mechanics-log-plugin/">**arcDPS mechanics**</a>
- <a href="https://github.com/megai2/d912pxy">**d912pxy**</a>
- <a href="https://github.com/Friendly0Fire/GW2Radial">**GW2Radial**</a>


#### Issues
If something doesn't work, please open an issue describing the problem and include what you were trying to do, the error message (if applicable), and a copy of your config.ini file.

### Uses

- <a href="https://www.newtonsoft.com/json">JSON.Net</a> 
- [Mvvm Light](http://www.mvvmlight.net/) 

#### [To-Do](https://docs.google.com/document/d/158CAGSGr-tgw4eVIxfYCZMLeeFDtlt89Elgq8K3b7tk/edit?usp=sharing)

&nbsp;

###### Disclaimer: I am not affiliated with any of the add-ons shown here or their authors, nor am I affiliated with ArenaNet. All trademarks, registered trademarks, logos, etc are the property of their respective owners. You use this tool at your own risk. If you are the author of one of the add-ons and wish for your add-on to be removed from the options for whatever reason, <a href="mailto:fmmmlee@gmail.com">send an email</a> or send an in-game mail to admiralnerf.1853 and I'll reply and take care of the issue as soon as I can.
