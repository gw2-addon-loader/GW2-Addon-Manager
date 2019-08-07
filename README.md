# GW2-Updater-Scripts
A few convenience scripts to update some Guild Wars 2 add-ons without having to manually go to each website, check version numbers, download and rename dlls, etc.

## Requirements:
- **Windows**
- **Powershell**

### Currently Supported Add-Ons:
- <a href="https://github.com/megai2/d912pxy">**d912pxy**</a>
- <a href="https://www.deltaconnected.com/arcdps/">**arcDPS**</a>
- <a href="https://github.com/Friendly0Fire/GW2Radial">**GW2Radial**</a>

### Using the Scripts
They should be plug-and-play, assuming that you have installed the add-on that a script is intended to update and you understand the **Versions** and **Requirements** sections of this Readme. Just run any of the .ps1 scripts in Powershell and it should check for updates (and perform them) for the add-on indicated in the filename. If things don't work, please make an issue describing the problem and include the error message (if applicable) and what add-ons you have installed.

### The Versions
- **Standalone**: You should be able to download and use a single .ps1 file from this folder and have it work properly. To set naming preferences, game path, etc, you need to edit the script itself.
- **Configurable**: The scripts in this folder rely on the dll_config.ini file. They should still work independently of each other, but they all require the config file. Currently, the config file is just an easy way to choose how to name the respective dlls of each add-on.

#### To-Do:
- Add "game path" section to config file - currently is hardcoded individually in each script to be C:\Program Files\Guild Wars 2, but some may have GW2 installed elsewhere and this would be useful in those cases
- Make one head script to run all the individual scripts, so all add-ons can be updated by running the head script
- See if the above head script can include a command that schedules it to run every so often in order for it to be an auto-check for updates


&nbsp;

###### Disclaimer: I am not affiliated with any of the add-ons shown here or their authors. If you are the author of one of the add-ons and wish for the update script for your add-on to be removed for whatever reason, <a href="mailto:fmmmlee@gmail.com">send an email</a> or send an in-game mail to admiralnerf.1853 and I'll reply and take care of the issue as soon as I can.
