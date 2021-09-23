using System;
using System.Collections.Generic;

namespace GW2_Addon_Manager
{
    [Obsolete]
    public record OldConfig(
        string application_version,
        string loader_version,
        string bin_folder,
        bool isupdate,
        bool launch_game,
        string game_path,
        string exe_name,
        string current_addon_list, // hash of the master branch
        Dictionary<string, bool> default_configuration,
        Dictionary<string, string> version,
        Dictionary<string, string> installed,
        Dictionary<string, bool> disabled);
}