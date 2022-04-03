using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace BLRP_FRAMEWORK
{
    public class Main : BaseScript
    {
        public static bool EnableHUD = false;
        public static bool isAdmin = false;
        public static bool isCop = false;

        public Main()
        {
            //Trigger Events
            TriggerServerEvent("BLRP_FRAMEWORK:GetPlayerSettings");

            //Load Utilities
            Utilities.Commands.RegisterCommands();

            //Discord
            Tick += SetDiscord;
            Tick += SaveData;
            Tick += DrawHUDS;
        }

        private static async Task SaveData()
        {
            await Delay(60000 * 5); //5 minutes until save

            //Save player data
            TriggerServerEvent("BLRP_FRAMEWORK:SavePlayerData");

            //Save player money
            TriggerServerEvent("BLRP_FRAMEWORK:SavePlayerMoney", CharacterSystem.CharacterMain.Money);

            //Save player last character (if it exist)
            if (CharacterSystem.CharacterMain.Name != null){string name = CharacterSystem.CharacterMain.Name;int money = CharacterSystem.CharacterMain.Money;TriggerServerEvent("BLRP_FRAMEWORK:SaveLastCharacter", name, money);}
        }

        private static async Task DrawHUDS()
        {
            //Speedometer HUD
            if (Functions.Speedometer.DrawSpeedometer)
            {
                Functions.Speedometer.DrawSpeed();
            }

            //Duty HUD
            if (isCop)
            {
                API.SetTextScale(0.6f, 0.6f);
                API.SetTextFont(7);
                API.SetTextProportional(true);
                API.SetTextColour((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue);
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.AddTextComponentString($"On Duty");
                API.DrawText(0.925f, 0.05f);
                API.EndTextComponent();
            }
        }

        private static async Task SetDiscord()
        {
            API.SetDiscordAppId("932033028488855602");
            API.SetRichPresence("Playing BlueLine Framework");
            API.SetDiscordRichPresenceAsset("logo");
            API.SetDiscordRichPresenceAction(0, "Download Now", "https://sites.google.com/view/bluelineframework");
        }
    }
}
