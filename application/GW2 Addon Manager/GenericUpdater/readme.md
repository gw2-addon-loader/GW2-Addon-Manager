### Description

The generic updater is intended to perform updates for any number of addons based solely on the content of each one's `update.yaml` file.﻿

There are many steps to performing an addon update, including checking versions against the user's `config.yaml` file (changed from the config.ini file used in 0.4.x and earlier), downloading an addon from a url, and installing and configuring its plugin - all steps that are to be performed according to the data provided in the addon's `update.yaml` to streamline application logic, improve extensibility, and remove boilerplate code.

This folder contains files for the generic updater.

How it will most likely work eventually:

- Application checks for and downloads latest release from approved-addons repo into /addons/
- Application reads from /addons/ and converts each update.yaml into an AddonInfo object which goes into a List
- UI displays information based on each item in this list
  - List of addons with checkboxes
  - Description, Developer, Links, etc. for the selected addon
- If an item is checked off in a checkboxlist in the UI and UPDATE is clicked, an instance of GenericUpdater is created for the addon corresponding to that checkbox and thataddon.Update() is run for all selected addons
- this update process generally involves downloading an addon to (game folder)/addons/(addon name)/, as well as either copying its update.yaml file to that folder or having it included in the release (for the addon loader to read from)
- During this process a list of chainloaded addons is recorded, and afterwards any chainloaded plugin configuration occurs
- Any final file IO ops (e.g. copying d3d9.dll for launchbuddy users)
- Update process is finished