# GW2-UOAOM [![Current Version](https://img.shields.io/badge/version-0.3.0-blue)](https://github.com/fmmmlee/GW2-Addon-Manager/releases)
##### Guild Wars 2 Unofficial Add-On Manager
A tool to install and update some Guild Wars 2 add-ons without having to manually visit each website, check version numbers, download and rename dlls, etc.

### Application Requirements:
- **Windows**
- **.NET Framework** (included in Windows 10)

### Currently Supported Add-Ons:
- <a href="https://github.com/megai2/d912pxy">**d912pxy**</a>
- <a href="https://www.deltaconnected.com/arcdps/">**arcDPS**</a>
- <a href="https://github.com/Friendly0Fire/GW2Radial">**GW2Radial**</a>

### Getting Started
Make sure your system meets the **Requirements** and that you don't have any unsupported add-ons (I hope to expand the list in the near future). Don't run the program while GW2 or the GW2 launcher is running, as the program needs to access files that are locked while the game is active.

- Download the ![latest release](https://github.com/fmmmlee/GW2-Addon-Manager/releases)
- Extract the zip file to any location you like
- Run "GW2 Addon Manager.exe" (see below for info about the first run)

A snapshot of the opening UI screen is here:

![GW2-UOAOM-v0.3.0-UI](https://user-images.githubusercontent.com/30479162/63399630-f598a880-c385-11e9-9367-d022eca3615a.JPG)

#### First Time Setup
Make sure that the game path is set correctly (the default is C:\Program Files\Guild Wars 2). The first time GW2-UOAOM is run, **all selected add-ons will be redownloaded**. See release page for details. Subsequent runs should only download new files if your version of an add-on is not the same as their latest release.

#### Issues
If something doesn't work, please open an issue describing the problem and include the error message (if applicable) as well as what add-ons you have installed.

### Uses
- <a href="https://www.newtonsoft.com/json">JSON.Net</a> (bundled with executable)

#### To-Do:
- add button to create start menu shortcut
- add settings panel with options to manually set .dll filenames and override the auto-naming
- add option for add-on delete to resolve dll chainloading conflicts that may be created by the deletion
- add option to clear config.ini and start anew (for if it gets corrupted or messed up somehow)
- split the powershell scripts into a separate repository, as they're no longer a focus

&nbsp;

###### Disclaimer: I am not affiliated with any of the add-ons shown here or their authors, nor am I affiliated with ArenaNet. All trademarks, registered trademarks, logos, etc are the property of their respective owners. You use this tool at your own risk. If you are the author of one of the add-ons and wish for your add-on to be removed from the options for whatever reason, <a href="mailto:fmmmlee@gmail.com">send an email</a> or send an in-game mail to admiralnerf.1853 and I'll reply and take care of the issue as soon as I can.
