using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace BLRP_FRAMEWORK.CharacterSystem
{
    public class CharacterEvents : BaseScript
    {

        public CharacterEvents()
        {
            //Creation Events
            EventHandlers["BLRP_FRAMEWORK:CharacterCreated"] += new Action<string, int, string>(CharacterCreated);
            EventHandlers["BLRP_FRAMEWORK:CharacterCreationError"] += new Action<string>(CharacterCreationError);

            //Update Events
            EventHandlers["BLRP_FRAMEWORK:CharacterUpdated"] += new Action<string, string, string, string, string, string>(CharacterUpdated);

            //Functions
            EventHandlers["BLRP_FRAMEWORK:GiveIDLocal"] += new Action<string, int, Vector3, bool>(GetLocalID);
            EventHandlers["BLRP_FRAMEWORK:GiveCCWLocal"] += new Action<string, int, Vector3, bool>(GetLocalCCW);

            //Vehicle Events
            EventHandlers["BLRP_FRAMEWORK:VehicleRegistered"] += new Action<string>(VehicleRegistered);
        }

        private void VehicleRegistered(string plate)
        {
            Screen.ShowNotification($"~g~[SUCCESS]~w~ Your plate, ~b~{plate}~w~ has been registered");
        }

        private void GetLocalID(string Name, int ped, Vector3 Position, bool useNUI)
        {
            //Get player coords
            Vector3 playerCoords = Game.Player.Character.Position;

            //Get distance
            float distance = World.GetDistance(playerCoords, Position);

            //Check distance
            if (distance <= 5f)
            {
                if (useNUI)
                {
                    //Trigger the NUI Event
                    TriggerEvent("BLFW_IDCARD:GiveNUICard", Name);
                }
                else
                {
                    //Show Identification
                    API.SetNotificationTextEntry("STRING");
                    API.SetNotificationColorNext(4);
                    API.AddTextComponentString($"~y~Name: ~b~{Name}");
                    API.SetTextScale(0.5f, 0.5f);
                    API.SetNotificationMessage("CHAR_DEFAULT", "CHAR_DEFAULT", false, 0, "State of San Andreas", "~o~Identification Card");
                    API.DrawNotification(true, false);
                }
            }
        }

        private void GetLocalCCW(string Name, int ped, Vector3 Position, bool useNUI)
        {
            //Get player coords
            Vector3 playerCoords = Game.Player.Character.Position;

            //Get distance
            float distance = World.GetDistance(playerCoords, Position);

            //Check distance
            if (distance <= 5f)
            {
                if (useNUI)
                {
                    //Trigger the NUI Event
                    TriggerEvent("BLFW_WEAPONCARD:GiveNUICard", Name);
                }
                else
                {
                    //Show Identification
                    API.SetNotificationTextEntry("STRING");
                    API.SetNotificationColorNext(4);
                    API.AddTextComponentString($"~y~Name: ~b~{Name}");
                    API.SetTextScale(0.5f, 0.5f);
                    API.SetNotificationMessage("CHAR_DEFAULT", "CHAR_DEFAULT", false, 0, "State of San Andreas", "~o~Weapon Permit Card");
                    API.DrawNotification(true, false);
                }
            }
        }

        private void CharacterCreationError(string errorMessage)
        {
            Screen.ShowNotification(errorMessage);
        }

        private void CharacterUpdated(string firstname, string lastname, string money, string job, string license, string citations)
        {
            CharacterMain.Name = firstname + " " + lastname;
            CharacterMain.Money = int.Parse(money);
            CharacterMain.Job = job;
            Screen.ShowNotification("~g~[SUCCESS]~w~ Your character has been updated!");
        }

        private void CharacterCreated(string name, int money, string job)
        {
            //Set Name
            CharacterMain.Name = name;

            //Set Money
            CharacterMain.Money = money;

            //Set Job
            CharacterMain.Job = job;

            //Set police job
            if (job == "police")
            {
                Main.isCop = true;
            }
            else
            {
                Main.isCop = false;
            }

            //Show notification
            Screen.ShowNotification($"Your new character, ~b~{name}~w~, has been created!");
        }
    }
}
