using System;

namespace GW2_Addon_Manager
{
    [Serializable]
    public record AddonState(string Name, string Version, bool Installed, bool Disabled);
}