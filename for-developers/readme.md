#### How-To

Include the file update.yaml in the top-level directory of your release. When a user downloads your add-on using GW2 UOAOM, this file will be copied into the application resources and supply information both for plugin configuration and to display to the user. See the example update.yaml file for more details.

Before a user downloads your release, information about your add-on will come from this file in the folder named after your addon: update-placeholder.yaml. It comes bundled with the application and will not be read if update.yaml is present.