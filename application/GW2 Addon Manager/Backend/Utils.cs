using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2_Addon_Manager
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public delegate void UpdateMessageChangedEventHandler(object sender, string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pct"></param>
    public delegate void UpdateProgressChangedEventHandler(object sender, int pct);

    public static class Constants
    {
        public const string AddonFolder = "resources\\addons";
    }

    public static class Utils
    {
        public static TValue? GetNull<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : class
        {
            return dict.TryGetValue(key, out var val) ? val : null;
        }
    }
}
