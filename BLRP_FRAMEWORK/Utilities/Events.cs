using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;

namespace BLRP_FRAMEWORK.Utilities
{
    public class Events : BaseScript
    {
        private static bool infoLoaded = false;
        public Events()
        {
            //Base
            EventHandlers["onResourceStart"] += new Action<string>(OnResourceStart);
            EventHandlers["onResourceStop"] += new Action<string>(OnResourceStopped);
            EventHandlers["playerSpawned"] += new Action(playerSpawned);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);

            //Generic
            EventHandlers["BLRP_FRAMEWORK:ErrorMessage"] += new Action<string>(ErrorMessage);
            EventHandlers["BLRP_FRAMEWORK:ClearChat"] += new Action(ClearChatGlobal);

            //Player
            EventHandlers["BLRP_FRAMEWORK:SetPlayerInfo"] += new Action<string, string, Vector3, string, string, string>(SetPlayerInfo);
            EventHandlers["BLRP_FRAMEWORK:UpdatedJobNotification"] += new Action<string>(UpdatedJobNotification);
            EventHandlers["BLRP_FRAMEWORK:LoadPlayerSettings"] += new Action<bool>(RetrievePlayerSettings);

            //Impound
            EventHandlers["BLRP_FRAMEWORK:ImpoundNotification"] += new Action<string, string>(ShowImpoundNotification);
            EventHandlers["BLRP_FRAMEWORK:AlreadyImpoundedNotification"] += new Action<string, string>(ShowImpoundErrorNotification);

            //Tickets
            EventHandlers["BLRP_FRAMEWORK:TicketIssuedNotification"] += new Action<string, string, string>(TicketIssuedNotification);
            EventHandlers["BLRP_FRAMEWORK:TicketRecievedNotification"] += new Action<string, string, string>(TicketRecievedNotification);

            //Menu
            EventHandlers["BLRP_FRAMEWORK:SendPlayerList"] += new Action<List<dynamic>, List<dynamic>>(RetrievePlayerList);
            EventHandlers["BLRP_FRAMEWORK:GetImpoundedPlateList"] += new Action<List<dynamic>, List<dynamic>>(RetrieveImpoundedPlates);
        }

        private static void OnResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;
            TriggerServerEvent("BLRP_FRAMEWORK:GetPlayerInfo");
        }

        private static void OnResourceStopped(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;
            TriggerServerEvent("BLRP_FRAMEWORK:SavePlayerData");
        }

        private static void playerSpawned()
        {
            if (Game.Player.Character.Exists())
            {
                if (infoLoaded == false)
                {
                    TriggerServerEvent("BLRP_FRAMEWORK:GetPlayerInfo");
                }
            }         
        }

        private void ClearChatGlobal()
        {
            TriggerEvent("chat:clear");
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 255, 0, 0 },
                multiline = true,
                args = new[] { "Chat has been cleared" }
            });
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            
        }

        private void ErrorMessage(string errorMessage)
        {
            Screen.ShowNotification($"~r~[ERROR]~w~ {errorMessage}");
        }

        private static void RetrievePlayerSettings(bool EnableHUD)
        {
            Main.EnableHUD = EnableHUD;
        }

        private static void SetPlayerInfo(string job, string admin, Vector3 LastPosition, string firstname, string lastname, string money)
        {
            //Spawn last position
            Game.Player.Character.Position = LastPosition;

            //Set Name
            CharacterSystem.CharacterMain.Name = $"{firstname} {lastname}";

            //Set Money
            CharacterSystem.CharacterMain.Money = int.Parse(money);

            //Set Job
            CharacterSystem.CharacterMain.Job = job;
            
            //Set police job
            if (job == "police")
            {
                Main.isCop = true;
            }

            //Set admin
            if (admin == "true")
            {
                Main.isAdmin = true;
            }
        }

        private static void ShowImpoundNotification(string plate, string model)
        {
            Game.Player.Character.CurrentVehicle.Delete();
            Screen.ShowNotification($"~g~[SUCCESS]~w~ Vehicle has been impounded ~n~~y~Plate: ~b~{plate}~n~~y~Model: ~b~{model}");
            TriggerServerEvent("BLRP_FRAMEWORK:GetImpoundedPlates");
        }

        private static void ShowImpoundErrorNotification(string plate, string model)
        {
            Screen.ShowNotification($"~r~[ERROR]~w~ Vehicle has already been impounded ~n~~y~Plate: ~b~{plate}~n~~y~Model: ~b~{model}");
        }

        private static void TicketIssuedNotification(string recipient, string amount, string violation)
        {
            Screen.ShowNotification($"You have issued a ticket to ~b~{recipient} ~w~for ~y~{violation}~w~ with a fine of ~g~{amount}");
        }

        private static void TicketRecievedNotification(string officer, string amount, string violation)
        {
            //Display notification
            Screen.DisplayHelpTextThisFrame($"You have been issued a ticket from ~b~{officer} ~w~for ~y~{violation}~w~ with a fine of ~g~{amount}");

            //Get the amount without dollar sign
            string simpleAmount = amount.Remove(0, 1);
            string finalSimpleAmount = simpleAmount.Replace(",", "");
            int ticketAmount = int.Parse(finalSimpleAmount);

            //Remove the money
            CharacterSystem.CharacterMain.Money -= ticketAmount;

            //Save the new balance
            TriggerServerEvent("BLRP_FRAMEWORK:SaveCharacterMoney", CharacterSystem.CharacterMain.Name, CharacterSystem.CharacterMain.Money);
        }

        private static void UpdatedJobNotification(string job)
        {
            Screen.ShowNotification($"Your job has been changed to ~b~{job}");

            if (job == "police")
            {
                Main.isCop = true;
            }
            else
            {
                Main.isCop = false;
            }
        }

        private static void RetrievePlayerList(List<dynamic> playerList, List<dynamic> playerNameList)
        {
            Menus.TicketMenu.playerList.Clear();
            Menus.TicketMenu.playerNameList.Clear();
            foreach (var player in playerList)
            {
                Menus.TicketMenu.playerList.Add(player.ToString());
            }
            foreach (var player in playerNameList)
            {
                Menus.TicketMenu.playerNameList.Add(player.ToString());
            }
            Menus.TicketMenu.mainMenu.RefreshIndex();
            Menus.TicketMenu._menuPool.RefreshIndex();
            Menus.TicketMenu.mainMenu.Visible = !Menus.TicketMenu.mainMenu.Visible;
        }

        private static void RetrieveImpoundedPlates(List<dynamic> plates, List<dynamic> models)
        {
            Menus.ImpoundMenu.impoundedPlates.Clear();
            Menus.ImpoundMenu.impoundedModels.Clear();
            if (plates.Count == 0)
            {
                Menus.ImpoundMenu.impoundedPlates.Add("No Impounded Vehicles");
            }
            else
            {
                foreach (var plate in plates)
                {
                    Menus.ImpoundMenu.impoundedPlates.Add(plate.ToString());
                }
            }
            
            if (models.Count == 0)
            {
                Menus.ImpoundMenu.impoundedModels.Add("No Impounded Vehicles");
            }
            else
            {
                foreach (var plate in models)
                {
                    Menus.ImpoundMenu.impoundedModels.Add(plate.ToString());
                }
            }
            Menus.ImpoundMenu.mainMenu.RefreshIndex();
            Menus.ImpoundMenu._menuPool.RefreshIndex();
            Menus.ImpoundMenu.mainMenu.Visible = !Menus.ImpoundMenu.mainMenu.Visible;
        }
    }
}
