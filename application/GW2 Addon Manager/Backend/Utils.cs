using System;
using System.Collections.Generic;
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
}
