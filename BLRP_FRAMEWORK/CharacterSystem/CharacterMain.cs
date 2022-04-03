using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLRP_FRAMEWORK.CharacterSystem
{
    public class CharacterMain : BaseScript
    {
        public static string Name;
        public static int Money;
        public static string Job;

        public CharacterMain()
        {
            //Commands
            API.RegisterCommand("giveid", new Action(GiveID), false);
            API.RegisterCommand("giveccw", new Action(GiveCCW), false);
            API.RegisterCommand("register-vehicle", new Action(RegisterVehicle), false);
            API.RegisterCommand("newcharacter", new Action<int, List<object>, string>((source, args, raw) =>
            {
                string firstname = args[0].ToString();
                string lastname = args[1].ToString();
                string money = args[2].ToString();
                string job = args[3].ToString();
                string license = args[4].ToString();
                TriggerServerEvent("BLRP_FRAMEWORK:CreateNewCharacter", firstname, lastname, money, job, license, "0");
            }), false);
            API.RegisterCommand("updatecharacter", new Action<int, List<object>, string>((source, args, raw) =>
            {
                string firstname = args[0].ToString();
                string lastname = args[1].ToString();
                string money = args[2].ToString();
                string job = args[3].ToString();
                string license = args[4].ToString();
                TriggerServerEvent("BLRP_FRAMEWORK:UpdateCharacter", firstname, lastname, money, job, license, "0");
            }), false);

            //Chat Suggestions
            TriggerEvent("chat:addSuggestion", "/newcharacter", "Create a new character", new[]
            {
                new { name="First Name", help="" },
                new { name="Last Name", help="" },
                new { name="Money", help="" },
                new { name="Job", help="" },
                new { name="License Status", help="" }
            });
            TriggerEvent("chat:addSuggestion", "/updatecharacter", "Update existing character", new[]
            {
                new { name="First Name", help="" },
                new { name="Last Name", help="" },
                new { name="Money", help="" },
                new { name="Job", help="" },
                new { name="License Status", help="" }
            });

            //Tick
            Tick += DrawHUD;
        }

        private static void GiveID()
        {
            if (Name == null)
            {
                //Notify the player that they do not have an active character
                Screen.ShowNotification("~r~[ERROR]~w~ You do not have an active character");
            }
            else
            {
                //Get the server ID of the player
                int myID = API.GetPlayerServerId(Game.Player.Character.Handle);

                //Sends the event and the ID of the player to the server
                TriggerServerEvent("BLRP_FRAMEWORK:GiveID", myID, Name, Game.Player.Character.Position);
            }
        }

        private static void GiveCCW()
        {
            if (Name == null)
            {
                //Notify the player that they do not have an active character
                Screen.ShowNotification("~r~[ERROR]~w~ You do not have an active character");
            }
            else
            {
                //Gets the server ID of the player
                int myID = API.GetPlayerServerId(Game.Player.Character.Handle);

                //Trigger the event and the ID of the player to the server
                TriggerServerEvent("BLRP_FRAMEWORK:GiveCCW", myID, Name, Game.Player.Character.Position);
            }
        }

        private static void RegisterVehicle()
        {
            //Check to see if player is in vehicle
            if (Game.Player.Character.IsInVehicle())
            {
                //Sets the current vehicle
                Vehicle currentVehicle = Game.Player.Character.CurrentVehicle;

                //Sets the vehicle properties (moderl, class, plate)
                string model = currentVehicle.DisplayName;
                string modelName = currentVehicle.LocalizedName;
                string className = currentVehicle.ClassType.ToString();
                string plate = currentVehicle.Mods.LicensePlate;

                //Checks if the name is null or invalid
                if (Name == null)
                {
                    //Set the owner unknown
                    string owner = "Unknown";

                    //Trigger the register vehicle server event
                    TriggerServerEvent("BLRP_FRAMEWORK:RegisterVehicle", modelName, className, plate, owner);
                }
                else
                {
                    //Sets the owner to the name of the player
                    string owner = Name;

                    //Trigger the register vehicle server event
                    TriggerServerEvent("BLRP_FRAMEWORK:RegisterVehicle", modelName, className, plate, owner);
                }
            }
            else
            {
                //Shows the error message that they are not in a vehicle
                Screen.ShowNotification("~r~[ERROR]~w~ You are not in a vehicle");
            }
        }

        private static async Task DrawHUD()
        {
            if (Main.EnableHUD)
            {
                //Draws current player name
                API.SetTextScale(0.4f, 0.4f);
                API.SetTextFont(7);
                API.SetTextProportional(true);
                API.SetTextColour((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue);
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.AddTextComponentString($"Name: ~b~{Name}");
                API.DrawText(0.01f, 0.05f); //0.035 DIFFERENCE WITH SET FONT AND SCALE
                API.EndTextComponent();

                //Draws current money balance
                API.SetTextScale(0.4f, 0.4f);
                API.SetTextFont(7);
                API.SetTextProportional(true);
                API.SetTextColour((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue);
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.AddTextComponentString($"Money: ~g~${Money.ToString()}");
                API.DrawText(0.01f, 0.085f); //0.035 DIFFERENCE WITH SET FONT AND SCALE
                API.EndTextComponent();

                //Draws current job
                API.SetTextScale(0.4f, 0.4f);
                API.SetTextFont(7);
                API.SetTextProportional(true);
                API.SetTextColour((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue);
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.AddTextComponentString($"Job: ~o~{Job}");
                API.DrawText(0.01f, 0.12f); //0.035 DIFFERENCE WITH SET FONT AND SCALE
                API.EndTextComponent();
            }
        }
    }
}
