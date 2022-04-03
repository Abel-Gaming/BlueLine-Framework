using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLRP_FRAMEWORK.Utilities
{
    public class PoliceStations : BaseScript
    {
        public static List<Vector3> DutyToggleLocations = new List<Vector3>();
        public static List<Vector3> ImpoundLocations = new List<Vector3>();

        public PoliceStations()
        {
            //On Load
            AddImpoundLocations();

            //Get Config Settings
            TriggerServerEvent("BLRP_FRAMEWORK:GetAddonConfig");

            //Events
            EventHandlers["BLRP_FRAMEWORK:ReturnAddonConfig"] += new Action<bool, bool>(AddStationLocations);

            //Tick
            Tick += DrawMarkers;
            Tick += CheckMarkers;

            //Draw Blips
            foreach (Vector3 location in ImpoundLocations)
            {
                int Blip = API.AddBlipForCoord(location.X, location.Y, location.Z);
                API.SetBlipSprite(Blip, 68);
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentSubstringPlayerName("Police Impound");
                API.EndTextCommandSetBlipName(Blip);
            }
        }

        private static void AddStationLocations(bool UltrunzBCES, bool GabzMRPD)
        {
            // Sandy Shores
            if (UltrunzBCES)
            {
                DutyToggleLocations.Add(new Vector3(1857.9410f, 3696.9389f, 34.3273f)); //Sandy Shores (Ultrunz)
            }
            else
            {
                DutyToggleLocations.Add(new Vector3(1851.9086914063f , 3690.5207519531f, 34.276851654053f)); //Sandy Shores (Default)
            }

            // Mission Row PD
            if (GabzMRPD)
            {
                DutyToggleLocations.Add(new Vector3(458.24624633789f, -998.84973144531f, 30.68950843811f)); //Mission Row (Gabz)
            }
            else
            {
                DutyToggleLocations.Add(new Vector3(452.6f, -992.8f, 30.6f)); //Mission Row (Default)
            }
            
            // Add other department locations
            DutyToggleLocations.Add(new Vector3(-1108.0452880859f, -844.79168701172f, 19.316972732544f)); //Vespucci
            DutyToggleLocations.Add(new Vector3(-561.31182861328f, -132.60423278809f, 38.038520812988f)); //Rockford Hills
            DutyToggleLocations.Add(new Vector3(360.62683105469f, -1584.4100341797f, 29.291940689087f)); //Davis
            DutyToggleLocations.Add(new Vector3(619.98663330078f, 17.95099067688f, 87.900917053223f)); //Vinewood
            DutyToggleLocations.Add(new Vector3(826.45867919922f, -1290.0520019531f, 28.240653991699f)); //La Mesa
            DutyToggleLocations.Add(new Vector3(-447.64303588867f, 6008.9638671875f, 31.716390609741f)); //Paleto Bay
            DutyToggleLocations.Add(new Vector3(379.13827514648f, 791.98870849609f, 190.40768432617f)); //Beaver Bush (Park Ranger)

            //Draw Blips
            foreach (Vector3 location in DutyToggleLocations)
            {
                int Blip = API.AddBlipForCoord(location.X, location.Y, location.Z);
                API.SetBlipSprite(Blip, 60);
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentSubstringPlayerName("Police Station");
                API.EndTextCommandSetBlipName(Blip);
            }
        }

        private static void AddImpoundLocations()
        {
            ImpoundLocations.Add(new Vector3(436.83065795898f, -1012.0104980469f, 28.634592056274f)); //Mission Row PD
        }

        private static async Task DrawMarkers()
        {
            foreach (Vector3 location in DutyToggleLocations)
            {
                API.DrawMarker(1, location.X, location.Y, location.Z, 0.0f, 0.0f, 0.0f, 0.0f, 180.0f, 0.0f, 1.0f, 1.0f, 1.0f, 65, 150, 245, 100, false, true, 2, false, null, null, false);
            }

            foreach (Vector3 location in ImpoundLocations)
            {
                API.DrawMarker(1, location.X, location.Y, location.Z, 0.0f, 0.0f, 0.0f, 0.0f, 180.0f, 0.0f, 1.0f, 1.0f, 1.0f, 65, 150, 245, 100, false, true, 2, false, null, null, false);
            }
        }

        private static async Task CheckMarkers()
        {
            foreach (Vector3 location in DutyToggleLocations)
            {
                float Distance = World.GetDistance(Game.Player.Character.Position, location);
                if (Distance <= 1.5f)
                {
                    Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ to toggle duty");
                    if (Game.IsControlJustPressed(0, Control.Pickup))
                    {
                        if (Main.isCop)
                        {
                            Screen.ShowNotification("~g~[SUCCESS]~w~ You are now off Duty");
                            TriggerServerEvent("BLRP_FRAMEWORK:PoliceOffDuty");
                        }
                        else if (!Main.isCop)
                        {
                            Screen.ShowNotification("~g~[SUCCESS]~w~ You are now on Duty");
                            TriggerServerEvent("BLRP_FRAMEWORK:PoliceOnDuty");
                        }
                    }
                }
                else
                {

                }
            }

            foreach (Vector3 location in ImpoundLocations)
            {
                float Distance = World.GetDistance(Game.Player.Character.Position, location);
                if (Distance <= 1.0f)
                {
                    Screen.DisplayHelpTextThisFrame("Press ~INPUT_PICKUP~ to open impound menu");
                    if (Game.IsControlJustPressed(0, Control.Pickup))
                    {
                        if (Main.isCop)
                        {
                            TriggerServerEvent("BLRP_FRAMEWORK:GetImpoundedVehicles");
                        }
                        else if (!Main.isCop)
                        {
                            Screen.ShowNotification("~r~[ERROR]~w~ You are not a cop");
                        }
                    }
                }
                else
                {

                }
            }
        }
    }
}