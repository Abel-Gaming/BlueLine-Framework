using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace BLRP_FRAMEWORK.Functions
{
    public class Speedometer
    {
        public static bool DrawSpeedometer = true;

        public static void DrawSpeed()
        {
            //Get Speed
            float tempspeed = API.GetEntitySpeed(API.GetVehiclePedIsIn(API.GetPlayerPed(-1), false));

            //Convert to MPH
            float speed = (float)(tempspeed * 2.2369);

            if (Game.Player.Character.IsInVehicle())
            {
                //Draw Speed
                API.SetTextScale(0.35f, 0.35f);
                API.SetTextFont(0);
                API.SetTextProportional(true);
                API.SetTextColour((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue);
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.AddTextComponentString("~y~Speed: ~b~" + Math.Floor(speed) + " ~w~MPH");
                API.DrawText(0.1825f, 0.89f);
            }
        }
    }
}
