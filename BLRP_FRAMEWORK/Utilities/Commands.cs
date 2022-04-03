using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;

namespace BLRP_FRAMEWORK.Utilities
{
    public class Commands : BaseScript
    {
        public static void RegisterCommands()
        {
            //Police Commands
            API.RegisterCommand("impound", new Action(ImpoundVehicle), false);
            API.RegisterCommand("+ticketmenu", new Action(ToggleTicketMenu), false);
            API.RegisterCommand("-ticketmenu", new Action(EmptyFunction), false);
            API.RegisterCommand("ticketmenu", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (Main.isCop)
                {
                    TriggerServerEvent("BLRP_FRAMEWORK:GetPlayerList");
                }
                else
                {
                    Screen.ShowNotification("~r~[ERROR]~w~ You are not a cop");
                }
            }), false);

            //HUD Commands
            API.RegisterCommand("speedometer", new Action(ToggleSpeedometer), false);

            //Delete vehicle commands
            API.RegisterCommand("dv", new Action(DeleteVehicle), false);
            API.RegisterCommand("dvl", new Action(DeleteVehicleLast), false);
            API.RegisterCommand("dlv", new Action(DeleteLocalVehicles), false);

            //Data commands
            API.RegisterCommand("+savedata", new Action(ForceSaveData), false);
            API.RegisterCommand("setjob", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (Main.isAdmin)
                {
                    string ID = args[0].ToString();
                    string Job = args[1].ToString();
                    TriggerServerEvent("BLRP_FRAMEWORK:UpdatePlayerJobInfo", ID, Job);
                }
                else
                {
                    Screen.ShowNotification("~r~[ERROR]~w~ You are not an admin");
                }

            }), false);

            //Setting Commands
            API.RegisterCommand("togglehud", new Action(EnableHUD), false);

            //Key Mappings
            API.RegisterKeyMapping("+ticketmenu", "Open Ticket Menu", "KEYBOARD", "F4");

            //Suggestions
            TriggerEvent("chat:addSuggestion", "/setjob", "Set a player's job", new[]
            {
                new { name="ID", help="Server ID of player" },
                new { name="Job", help="Job to set to the player" }
            });
        }

        private static void DeleteVehicle()
        {
            if (Game.Player.Character.IsInVehicle())
            {
                //Deletes the current vehicle and shows a notification
                Game.Player.Character.CurrentVehicle.Delete();
                Screen.ShowNotification("~g~[SUCCESS]~w~ Vehicle has been deleted");
            }
            else
            {
                //Shows an error notification
                Screen.ShowNotification("~r~[ERROR]~w~ You are not in a vehicle");
            }
            
        }

        private static void DeleteLocalVehicles()
        {
            foreach (Vehicle car in World.GetAllVehicles())
            {
                if (World.GetDistance(Game.Player.Character.Position, car.Position) < 5f)
                {
                    car.Delete();
                }
            }
            Screen.ShowNotification("~g~[SUCCESS]~w~ Local vehicles have been deleted");
        }

        private static void ToggleSpeedometer()
        {
            //Sets the DrawSpeedometer boolean to the opposite of whatever it is
            Functions.Speedometer.DrawSpeedometer = !Functions.Speedometer.DrawSpeedometer;
        }

        private static void DeleteVehicleLast()
        {
            if (Game.Player.Character.LastVehicle == null)
            {
                //Do nothing
            }
            else
            {
                //Deletes the last vehicle and shows a notification
                Game.Player.Character.LastVehicle.Delete();
                Screen.ShowNotification("~g~[SUCCESS]~w~ Vehicle has been deleted");
            }

        }

        private static void ImpoundVehicle()
        {
            if (Game.Player.Character.IsInVehicle())
            {
                if (Main.isCop)
                {
                    // Get vehicle player is in (get both methods)
                    Vehicle currentVehicle = Game.Player.Character.CurrentVehicle;
                    int currentVehicleInt = currentVehicle.Handle;

                    // Get vehicle plate and model
                    string plate = currentVehicle.Mods.LicensePlate;
                    string model = currentVehicle.DisplayName;

                    // Get vehicle mods
                    string primarycolor = ((int)currentVehicle.Mods.PrimaryColor).ToString();
                    string secondarycolor = ((int)currentVehicle.Mods.SecondaryColor).ToString();

                    // Send data
                    TriggerServerEvent("BLRP_FRAMEWORK:ImpoundVehicle", plate, model, primarycolor, secondarycolor);
                }
                else
                {
                    //Shows an error notification that the player is not a cop
                    Screen.ShowNotification("~r~[ERROR]~w~ You are not a cop");
                }
            }
            else
            {
                //Shows an error notification that the player is not in a vehicle
                Screen.ShowNotification("~r~[ERROR]~w~ You are not in a vehicle");
            }
        }

        private static void ForceSaveData()
        {
            //Trigger the server event to save data (such as position)
            TriggerServerEvent("BLRP_FRAMEWORK:SavePlayerData");

            //Trigger the server event to save the player money
            TriggerServerEvent("BLRP_FRAMEWORK:SavePlayerMoney", CharacterSystem.CharacterMain.Money);

            //Trigger the event to save the player's last character if it exist
            if (CharacterSystem.CharacterMain.Name == null)
            {

            }
            else
            {
                string name = CharacterSystem.CharacterMain.Name;
                int money = CharacterSystem.CharacterMain.Money;
                TriggerServerEvent("BLRP_FRAMEWORK:SaveLastCharacter", name, money);
            }
        }

        private static void EnableHUD()
        {
            //Sets the EnableHUD bool opposite to what it already is
            Main.EnableHUD = !Main.EnableHUD;

            //Triggers the event to save the player settings to the EnableHUD bool
            TriggerServerEvent("BLRP_FRAMEWORK:UpdatePlayerSettings", Main.EnableHUD.ToString());
        }

        private static void ToggleTicketMenu()
        {
            //Checks to see if the player is able to open the ticket menu
            if (Main.isCop)
            {
                //If the player is a cop, trigger the event to get the server player list
                TriggerServerEvent("BLRP_FRAMEWORK:GetPlayerList");
            }
            else
            {
                //Shows error notification that the player is not a cop
                Screen.ShowNotification("~r~[ERROR]~w~ You are not a cop");
            }
        }

        private static void EmptyFunction()
        {
            //Does nothing
        }
    }
}


// CODE BELOW IS THE WORKING TICKETMENU CODE IN CASE THE PLAYERLIST DOES NOT WORK //

/*          
            API.RegisterCommand("ticketmenu", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (Main.isCop)
                {
                    Menus.TicketMenu.SelectedPlayerID = args[0].ToString();
                    Menus.TicketMenu.mainMenu.Visible = !Menus.TicketMenu.mainMenu.Visible;
                }
                else
                {
                    Screen.ShowNotification("~r~[ERROR]~w~ You are not a cop");
                }

            }), false);
*/