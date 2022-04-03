using System;
using MySqlConnector;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace BLRP_FRAMEWORK_SERVER
{
    public class Main : BaseScript
    {
        public Main()
        {
            //Load the config
            Load.Config();

            //Write that the resource has started to the log
            Debug.WriteLine("BlueLine Framework by Abel Gaming has been loaded");

            //Internal FiveM Events
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);

            //Commands
            API.RegisterCommand("clearchat", new Action(ClearChatGlobal), false);
        }

        private void ClearChatGlobal()
        {
            TriggerClientEvent("BLRP_FRAMEWORK:ClearChat");
        }

        private async void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();
            await Delay(0);

            var Identifier = player.Identifiers["license"];

            if (!string.IsNullOrEmpty(Identifier))
            {
                if (!GetPlayerExistDB(Identifier))
                {
                    Vector3 DefaultPosition = new Vector3(187.02220153809f, -950.75988769531f, 30.091932296753f);
                    Database.ExecuteInsertQuery($"INSERT INTO users (Identifier, Name, Admin, Job, PositionX, PositionY, PositionZ) VALUES ('{Identifier}', '{playerName}', 'false', 'unemployed', '{DefaultPosition.X}', '{DefaultPosition.Y}', '{DefaultPosition.Z}')");
                }
            }

            deferrals.done();

            //Close Connection
            Database.Connection.Close();
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            
        }

        private bool GetPlayerExistDB(string Identifier)
        {
            MySqlDataReader Result = Database.ExecuteSelectQuery($"SELECT Identifier FROM users WHERE Identifier = '{Identifier}'");

            bool GetPlayerExistDB = false;
            while (Result.Read())
            {
                GetPlayerExistDB = true;
            }
            Database.Connection.Close();

            return GetPlayerExistDB;
        }      
    }
}
