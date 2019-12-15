using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2_Addon_Manager.Tests
{
    class ConfigurationTests
    {
        //all this will need to be set up to run in a special folder --- or at least save the config_template.yaml
        //and then copy back after it finishes/terminates/throws an exception
        public void GamePath_SetsProperly()
        {
            Configuration.SetGamePath("C:\\Program Files\\Test\\Guild Wars 2");
            
        }
    }
}
