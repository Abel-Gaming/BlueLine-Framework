using System;
using MySqlConnector;
using CitizenFX.Core;

namespace BLRP_FRAMEWORK_SERVER.Events
{
    public class PlayerEvents : BaseScript
    {
        public PlayerEvents()
        {
            //Save Player Stuff
            EventHandlers["BLRP_FRAMEWORK:SavePlayerData"] += new Action<Player>(SavePlayerData);
            EventHandlers["BLRP_FRAMEWORK:SavePlayerMoney"] += new Action<Player, int>(SavePlayerMoney);

            //Update Player Stuff
            EventHandlers["BLRP_FRAMEWORK:UpdatePlayerAdminInfo"] += new Action<int, string>(UpdatePlayerAdminInfo);
            EventHandlers["BLRP_FRAMEWORK:UpdatePlayerJobInfo"] += new Action<int, string>(UpdatePlayerJobInfo);
            EventHandlers["BLRP_FRAMEWORK:UpdatePlayerSettings"] += new Action<Player, string>(UpdatePlayerSettings);

            //Get Player Stuff
            EventHandlers["BLRP_FRAMEWORK:GetPlayerInfo"] += new Action<Player>(GetPlayerInfo);
            EventHandlers["BLRP_FRAMEWORK:GetPlayerSettings"] += new Action<Player>(GetPlayerSettings);

            //Suggestions
            TriggerEvent("chat:addSuggestion", "/setjob", "Set a player's job", new[]
            {
                new { name="ID", help="Server ID of player" },
                new { name="Job", help="Job to set to the player" }
            });
        }

        private static void SavePlayerData([FromSource] Player player)
        {
            //Get Player Identifier
            var Identifier = player.Identifiers["license"];

            //Get last position
            Vector3 LastPosition = player.Character.Position;

            //Update Database
            Database.ExecuteUpdateQuery($"UPDATE users SET PositionX = '{LastPosition.X}' WHERE Identifier = '{Identifier}'");
            Database.ExecuteUpdateQuery($"UPDATE users SET PositionY = '{LastPosition.Y}' WHERE Identifier = '{Identifier}'");
            Database.ExecuteUpdateQuery($"UPDATE users SET PositionZ = '{LastPosition.Z}' WHERE Identifier = '{Identifier}'");

            //Debug
            Debug.WriteLine($"Saved position: {LastPosition.ToString()} for {player.Name}");
        }

        private static void SavePlayerMoney([FromSource] Player player, int Money)
        {
            //Get Player Identifier
            var Identifier = player.Identifiers["license"];

            //Update Database
            Database.ExecuteUpdateQuery($"UPDATE characters SET Money = '{Money}' WHERE Identifier = '{Identifier}'");
        }

        private void UpdatePlayerAdminInfo(int ID, string status)
        {
            //Close any database connection
            Database.Connection.Close();

            //Generate playerlist
            PlayerList Playerlist = new PlayerList();

            //Select our player
            Player player = Playerlist[ID];

            //Get identifier
            var Identifier = player.Identifiers["license"];
            
            //Update database
            Database.ExecuteUpdateQuery($"UPDATE users SET Admin = {status} WHERE Identifier = '{Identifier}'");

            //Close Connection
            Database.Connection.Close();
        }

        private void UpdatePlayerJobInfo(int ID, string job)
        {
            //Close any database connection
            Database.Connection.Close();

            //Generate playerlist
            PlayerList Playerlist = new PlayerList();

            //Select our player
            Player player = Playerlist[ID];

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Update database
            Database.ExecuteUpdateQuery($"UPDATE users SET Job = '{job}' WHERE Identifier = '{Identifier}'");

            //Close Connection
            Database.Connection.Close();

            //Update database
            Database.ExecuteUpdateQuery($"UPDATE characters SET Job = '{job}' WHERE Identifier = '{Identifier}'");

            //Close Connection
            Database.Connection.Close();

            //Notify the player of the change
            player.TriggerEvent("BLRP_FRAMEWORK:UpdatedJobNotification", job);
        }

        private void UpdatePlayerSettings([FromSource] Player player, string EnableHUD)
        {
            //Close any Connection
            Database.Connection.Close();

            // Get player
            var Identifier = player.Identifiers["license"];

            // Check to see if the car is already impounded
            if (!DoesSettingFileExist(Identifier))
                {
                    //INSERT IF IT DOES NOT EXIST
                    Database.ExecuteInsertQuery($"INSERT INTO usersettings (Identifier, EnableHUD) VALUES ('{Identifier}', 'True')");

                    //Close Connection
                    Database.Connection.Close();
                }
                else
                {
                    //UPDATE IF IT EXIST
                    Database.ExecuteUpdateQuery($"UPDATE usersettings SET EnableHUD = '{EnableHUD}' WHERE Identifier = '{Identifier}'");

                    //Close Connection
                    Database.Connection.Close();
                }
            }

        private static void GetPlayerInfo([FromSource] Player player)
        {
            //Close any connection
            Database.Connection.Close();

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Reopen connection
            Database.Connection.Open();

            //Retrieve player info
            MySqlDataReader retrieveInfo = Database.ExecuteSelectQuery($"SELECT Job, Admin, PositionX, PositionY, PositionZ, LastCharacterFirstName, LastCharacterLastName FROM users WHERE Identifier = '{Identifier}'");
            string job = "";
            string admin = "";
            string xpos = "";
            string ypos = "";
            string zpos = "";
            string LastCharacterFirst = "";
            string LastCharacterLast = "";
            while (retrieveInfo.Read())
            {
                job = retrieveInfo["Job"].ToString();
                admin = retrieveInfo["Admin"].ToString();
                xpos = retrieveInfo["PositionX"].ToString();
                ypos = retrieveInfo["PositionY"].ToString();
                zpos = retrieveInfo["PositionZ"].ToString();
                LastCharacterFirst = retrieveInfo["LastCharacterFirstName"].ToString();
                LastCharacterLast = retrieveInfo["LastCharacterLastName"].ToString();
            }

            //Print to console
            Debug.WriteLine($"Loading character {LastCharacterFirst} {LastCharacterLast} for player {player.Name}");

            //Close connection
            Database.Connection.Close();

            //Open connection
            Database.Connection.Open();

            //Get character data
            MySqlDataReader retrievecharacterInfo = Database.ExecuteSelectQuery($"SELECT First_Name, Last_Name, Money, Job FROM characters WHERE Identifier = '{Identifier}' AND First_Name = '{LastCharacterFirst}' AND Last_Name = '{LastCharacterLast}'");
            string firstname = "";
            string lastname = "";
            string money = "";
            string characterJob = "";
            while (retrievecharacterInfo.Read())
            {
                firstname = retrievecharacterInfo["First_Name"].ToString();
                lastname = retrievecharacterInfo["Last_Name"].ToString();
                money = retrievecharacterInfo["Money"].ToString();
                characterJob = retrievecharacterInfo["Job"].ToString();
            }

            //Close connection
            Database.Connection.Close();

            //Convert Position to vector3
            float X = float.Parse(xpos);
            float Y = float.Parse(ypos);
            float Z = float.Parse(zpos);
            Vector3 lastPosition = new Vector3(X, Y, Z);

            //Send info back
            player.TriggerEvent("BLRP_FRAMEWORK:SetPlayerInfo", characterJob, admin, lastPosition, firstname, lastname, money);
        }

        private void GetPlayerSettings([FromSource] Player player)
        {
            //Close any connection
            Database.Connection.Close();

            // Get player
            var Identifier = player.Identifiers["license"];

            //Retrieve the settings
            MySqlDataReader retrievedhudsettings = Database.ExecuteSelectQuery($"SELECT EnableHUD FROM usersettings WHERE Identifier = '{Identifier}'");
            string hudstatus = "";
            bool EnableHUD;
            while (retrievedhudsettings.Read())
            {
                hudstatus = retrievedhudsettings["EnableHUD"].ToString();
            }
            if (hudstatus == "True")
            {
                EnableHUD = true;
            }
            else
            {
                EnableHUD = false;
            }

            //Close connection
            Database.Connection.Close();

            //Trigger event
            player.TriggerEvent("BLRP_FRAMEWORK:LoadPlayerSettings", EnableHUD);
        }

        private bool DoesSettingFileExist(string Identifier)
        {
            MySqlDataReader Result = Database.ExecuteSelectQuery($"SELECT Identifier FROM usersettings WHERE Identifier = '{Identifier}'");

            bool DoesPlayerHaveSettingFile = false;
            while (Result.Read())
            {
                DoesPlayerHaveSettingFile = true;
            }
            Database.Connection.Close();

            return DoesPlayerHaveSettingFile;
        }
    }
}
