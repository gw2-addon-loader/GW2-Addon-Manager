# TODO

### Updating Addons

- Add method to fetch latest [repo with list of compatible addons](https://github.com/gw2-addon-loader/Approved-Addons) (aim to have this in 1.1.0)
- version tracking for BuildPad

### UX

- If error occurs during one addon’s install process, display an error message and print to log file, then continue the update process for the other addons
- Toggle for send to recycle bin/permanently delete files

### Bugs

- Mechanics log and boontable not working for at least 2 users (see [discord channel](https://discord.gg/n2pCuCG))
- Apparently if the mechanics log plugin already exists in the user’s /addons/arcdps folder, an error is thrown. **This behavior should be consistent across all addons to simply overwrite without causing an error.**