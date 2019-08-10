# GW2-Addon-Updater
A tool to update some Guild Wars 2 add-ons without having to manually go to each website, check version numbers, download and rename dlls, etc.

## Requirements:
#### Scripts
- **Windows**
- **Powershell 3.0 or above**
#### Executable Application
- **Windows**
- **.NET Framework**

### Currently Supported Add-Ons:
- <a href="https://github.com/megai2/d912pxy">**d912pxy**</a>
- <a href="https://www.deltaconnected.com/arcdps/">**arcDPS**</a>
- <a href="https://github.com/Friendly0Fire/GW2Radial">**GW2Radial**</a>

### Using the Tool
Make sure your system meets the **Requirements** and that you have installed the add-ons in the list above (I'll add an option to specify which add-ons you have in the near future).
Running `update_all.bat` will check for updates for the addons and install them, if there are any. On the first run, some add-ons may be redownloaded even if they are up-to-date, as they do not include files that state their versions by default, so those files must be created. Subsequent runs should only download new files if your version of an add-on is not the same as the latest release.

If something doesn't work, please open an issue describing the problem and include the error message (if applicable) and what add-ons you have installed.

### Variants
- **Application**: This is the release variant (currently a pre-release, see the "releases" section of the repository for more details). It's currently a work in progress.
- **Scripts (Configurable)**: The scripts in this folder can be run individually or all at once, and each relies on the `dll_config.ini` file to get various settings used within the scripts. Using `update_all.bat` to perform all updates at once is recommended.
- **Scripts (Standalone)**: You should be able to download and run a single powershell script from this folder and have it work properly (assuming your script execution policy is set to allow them to run). To set naming preferences, game path, etc, you need to edit each individual script itself. These are meant more for testing or for those who don't care about an interface, or those who want to play with and edit the scripts without messing with a configuration file.

### Uses
- <a href="https://www.newtonsoft.com/json">JSON.Net</a> (bundled with executable)

#### To-Do:
- Make the application GUI look presentable
- Look into adding TACO, GW2Hook, and Reshade
- See if it's possible to schedule the full update to run every so often in order for it to be an auto-check for updates

&nbsp;

###### Disclaimer: I am not affiliated with any of the add-ons shown here or their authors. If you are the author of one of the add-ons and wish for the update script for your add-on to be removed for whatever reason, <a href="mailto:fmmmlee@gmail.com">send an email</a> or send an in-game mail to admiralnerf.1853 and I'll reply and take care of the issue as soon as I can.
