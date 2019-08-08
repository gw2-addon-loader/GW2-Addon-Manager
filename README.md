# GW2-Updater-Scripts
A few convenience scripts to update some Guild Wars 2 add-ons without having to manually go to each website, check version numbers, download and rename dlls, etc.

## Requirements:
- **Windows**
- **Powershell 3.0 or above**

### Currently Supported Add-Ons:
- <a href="https://github.com/megai2/d912pxy">**d912pxy**</a>
- <a href="https://www.deltaconnected.com/arcdps/">**arcDPS**</a>
- <a href="https://github.com/Friendly0Fire/GW2Radial">**GW2Radial**</a>

### Using the Scripts
They should be plug-and-play, assuming that you have installed the add-on that a script is intended to update and you understand the **Versions** and **Requirements** sections of this Readme. On the first run, some add-ons may be redownloaded even if they are up-to-date as they do not include files that state their versions by default and those files must be created.

If things don't work, please open an issue describing the problem and include the error message (if applicable) and what add-ons you have installed.

### The Versions
- **Standalone**: You should be able to download and use a single .ps1 file from this folder and have it work properly. To set naming preferences, game path, etc, you need to edit the script itself.
- **Configurable**: The scripts in this folder can be run individually or all at once. They rely on the `dll_config.ini` file to get various settings used within. Using `update_all.bat` to perform all updates at once is recommended.

#### To-Do:
- Add simple UI to `update_all.ps1` to have options to edit the fields in `dll_config.ini` and provide status reports on the updates.
- See if it's possible to schedule the full update to run every so often in order for it to be an auto-check for updates


&nbsp;

###### Disclaimer: I am not affiliated with any of the add-ons shown here or their authors. If you are the author of one of the add-ons and wish for the update script for your add-on to be removed for whatever reason, <a href="mailto:fmmmlee@gmail.com">send an email</a> or send an in-game mail to admiralnerf.1853 and I'll reply and take care of the issue as soon as I can.
