# TODO

### Updating Addons

- Add method to fetch latest [repo with list of compatible addons](https://github.com/gw2-addon-loader/Approved-Addons)
- Buildpad compatibility - scan folder for .dlls for filename

### UX

- If error occurs during one addon’s install process, display an error message and print to log file, then continue the update process for the other addons
- clean install button
- display conflicts/dependencies
- button to select game path using file explorer instead of pasting it in manually

### Bugs

- Apparently if the mechanics log plugin already exists in the user’s /addons/arcdps folder, an error is thrown. **This behavior should be consistent across all addons to simply overwrite without causing an error.**