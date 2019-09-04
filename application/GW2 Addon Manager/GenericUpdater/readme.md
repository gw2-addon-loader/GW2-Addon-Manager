### Description

The generic updater is intended to perform updates for any number of addons based solely on the content of each one's `update.yaml` file.﻿

There are many steps to performing an addon update, including checking versions against the user's `config.yaml` file (changed from the config.ini file used in 0.4.x and earlier), downloading an addon from a url, and installing and configuring its plugin - all steps that are to be performed according to the data provided in the addon's `update.yaml` to streamline application logic, improve extensibility, and remove boilerplate code.

This folder contains files for the generic updater.

How it will most likely work eventually:

- Application checks for and downloads latest release from approved-addons repo into (application folder)/addons
- Application reads from /addons and converts each update.yaml into an AddonInfo object which goes into a List
- Application UI displays information based on each item in this list
- If an item is checked off in a checkboxlist in the UI and UPDATE is clicked, an instance of GenericUpdater is created for the addon corresponding to that checkbox and thataddon.Update() is run
- During this process a list of chainloaded addons is recorded, and afterwards any chainloaded plugin configuration occurs
- Update process is finished