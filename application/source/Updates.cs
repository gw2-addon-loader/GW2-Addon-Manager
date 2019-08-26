using System.Threading.Tasks;
using System.Windows;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// <c>Updates</c> checks for, downloads, and installs updates for currently installed add-ons and installs new add-ons.
    /// </summary>
    class Updates
    {
        string game_path;

        string arc_name;
        string arc_templates_name;
        string d912pxy_name;
        string gw2radial_name;

        UpdatingViewModel view;

        /// <summary>
        /// This constructor sets the plugin configuration and performs a full update when called.
        /// </summary>
        /// <param name="view"></param>
        public Updates(UpdatingViewModel view)
        {
            this.view = view;
            getPreferences();
            Task.Run(() => Update()); //running the update method in the background so UI updates immediately
        }

        /// <summary>
        /// <c>getPreferences</c> reads the current application properties and determines the naming scheme for the plugin configuration specified.
        /// </summary>
        public void getPreferences()
        {
            dynamic config_obj = configuration.getConfig();
            game_path = config_obj.game_path;

            Application.Current.Properties["game_path"] = game_path;

            arc_name = config_obj.arcDPS;
            arc_templates_name = config_obj.arcDPS_buildTemplates;
            d912pxy_name = config_obj.d912pxy;
            gw2radial_name = config_obj.gw2Radial;

            //determining dll names
            if ((bool)Application.Current.Properties["ArcDPS"])
            {
                arc_name = "d3d9.dll";

                if ((bool)Application.Current.Properties["GW2Radial"])
                {
                    gw2radial_name = "d3d9_chainload.dll";
                    d912pxy_name = "d912pxy.dll";
                    //gw2hook_name = "ReShade64.dll";
                }
                else if ((bool)Application.Current.Properties["d912pxy"])
                {
                    d912pxy_name = "d3d9_chainload.dll";
                }
            }
            else if ((bool)Application.Current.Properties["GW2Radial"])
            {
                gw2radial_name = "d3d9.dll";
                d912pxy_name = "d912pxy.dll";
                //gw2hook_name = "ReShade64.dll";
            }
            else if ((bool)Application.Current.Properties["d912pxy"])
            {
                d912pxy_name = "d3d9.dll";
            }

            config_obj.installed.arcdps = arc_name;
            config_obj.installed.gw2radial = gw2radial_name;
            config_obj.installed.d912pxy = d912pxy_name;

            configuration.setConfig(config_obj);
            /*else if ((bool)Application.Current.Properties["gw2hook"])
            {
                gw2hook_name = "d3d9.dll";
            }*/

        }

        /// <summary>
        /// <c>Update</c> reads the application properties and calls the update function from the relevant class to perform the update for a given addon.
        /// </summary>
        public async void Update()
        {
            dynamic config_obj = configuration.getConfig();

            /* Will this lead to stack overflow w/ too many recursions? */
            /* Note: I have this weird if-else-if-else w/ application properties due to the execution going to the end and setting
            the label to completed and stuff while the async functions were still running before. I've made changes since then but
            I'm not sure if they fix that issue. Not really a priority atm but may test it when I feel like diving down that rabbit
            hole again */

            if ((bool)Application.Current.Properties["ArcDPS"] && !(bool)config_obj.disabled.arcdps)
            {
                arcdps arc = new arcdps(arc_name, arc_templates_name, view);
                await arc.update();
                Update();
            }
            else if ((bool)Application.Current.Properties["GW2Radial"] && !(bool)config_obj.disabled.gw2radial)
            {
                gw2radial radial = new gw2radial(gw2radial_name, view);
                await radial.update();
                Update();
            }
            else if ((bool)Application.Current.Properties["d912pxy"] && !(bool)config_obj.disabled.d912pxy)
            {
                d912pxy d912 = new d912pxy(d912pxy_name, view);
                await d912.update();
                Update();
            }
            /*else if ((bool)Application.Current.Properties["gw2hook"])
            {
                view.label = "Updating Gw2 Hook";
                Update();
            }*/
            else
            {
                view.label = "Complete";
                view.showProgress = 100;
                //enable "finish" button
                view.closeButtonEnabled = true;
            }
        }

        
    }
}
