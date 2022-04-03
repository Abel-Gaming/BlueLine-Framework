using System;
using System.Collections.Generic;
using CitizenFX.Core;
using MySqlConnector;

namespace BLRP_FRAMEWORK_SERVER.Events
{
    public class CharacterSystemEvents : BaseScript
    {
        public CharacterSystemEvents()
        {
            //Create Events
            EventHandlers["BLRP_FRAMEWORK:CreateNewCharacter"] += new Action<Player, string, string, string, string, string, string>(CreateNewCharacter);
            EventHandlers["BLRP_FRAMEWORK:RegisterVehicle"] += new Action<Player, string, string, string, string>(RegisterNewVehicle);

            //Update Events
            EventHandlers["BLRP_FRAMEWORK:UpdateCharacter"] += new Action<Player, string, string, string, string, string, string>(UpdateCharacter);

            //Save Events
            EventHandlers["BLRP_FRAMEWORK:SaveLastCharacter"] += new Action<Player, string, int>(SaveCharacterInfo);
            EventHandlers["BLRP_FRAMEWORK:SaveCharacterMoney"] += new Action<Player, string, int>(SaveCharacterMoney);

            //Get Events
            EventHandlers["BLRP_FRAMEWORK:GetSelectedCharacterInfo"] += new Action<Player, string>(GetCharacterInfo);

            //Function Events
            EventHandlers["BLRP_FRAMEWORK:GiveID"] += new Action<Player, int, string, Vector3>(GiveID);
            EventHandlers["BLRP_FRAMEWORK:GiveCCW"] += new Action<Player, int, string, Vector3>(GiveCCW);

            //Retrieving Events
            EventHandlers["BLRP_FRAMEWORK:GetPlayersCharacters"] += new Action<Player>(GetPlayerCharacters);

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
        }

        private void GetCharacterInfo([FromSource] Player player, string name)
        {
            //Debug
            Debug.WriteLine($"Getting character info for {name}");

            //Close any connection
            Database.Connection.Close();

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Split Full Name
            var names = name.Split(' ');
            string firstName = names[0];
            string lastName = names[1];

            //Open connection
            Database.Connection.Open();

            //Get character data
            MySqlDataReader retrievecharacterInfo = Database.ExecuteSelectQuery($"SELECT First_Name, Last_Name, Money, Job FROM characters WHERE Identifier = '{Identifier}' AND First_Name = '{firstName}' AND Last_Name = '{lastName}'");
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

            //Send info back
            player.TriggerEvent("BLRP_FRAMEWORK:GetCharacterInfoReturn", firstname, lastname, money, characterJob);
        }

        private void SaveCharacterMoney([FromSource] Player player, string name, int money)
        {
            //Get Identifier
            var Identifier = player.Identifiers["license"];

            //Get first and last name
            var names = name.Split(' ');
            string firstName = names[0];
            string lastName = names[1];

            //Save Money
            Database.ExecuteUpdateQuery($"UPDATE characters SET Money = '{money}' WHERE Identifier = '{Identifier}' AND First_Name = '{firstName}' AND Last_Name = '{lastName}'");

            //Close database connection
            Database.Connection.Close();
        }

        private void SaveCharacterInfo([FromSource] Player player, string name, int money)
        {
            //Get Identifier
            var Identifier = player.Identifiers["license"];
            var names = name.Split(' ');
            string firstName = names[0];
            string lastName = names[1];

            //Save Money
            Database.ExecuteUpdateQuery($"UPDATE characters SET Money = '{money}' WHERE Identifier = '{Identifier}' AND First_Name = '{firstName}' AND Last_Name = '{lastName}'");

            //Input Last Character to DB
            Database.ExecuteUpdateQuery($"UPDATE users SET LastCharacterFirstName = '{firstName}' WHERE Identifier = '{Identifier}'");
            Database.ExecuteUpdateQuery($"UPDATE users SET LastCharacterLastName = '{lastName}' WHERE Identifier = '{Identifier}'");

            //Close database connection
            Database.Connection.Close();

            //Print to console
            Debug.WriteLine($"Saved character: {firstName} {lastName}");
        }

        private void RegisterNewVehicle([FromSource] Player player, string model, string className, string plate, string owner)
        {
            //Get Identifier
            var Identifier = player.Identifiers["license"];

            //Check to see if the plate already exist
            if (!DoesPlateAlreadyExist(plate))
            {
                //Insert to database
                Database.ExecuteInsertQuery($"INSERT INTO vehicles (Identifier, Model_Name, Plate, Class_Type, Registered_To) VALUES ('{Identifier}', '{model}', '{plate}', '{className}', '{owner}')");

                //Close any database connection
                Database.Connection.Close();

                //Trigger Event
                player.TriggerEvent("BLRP_FRAMEWORK:VehicleRegistered", plate);
            }
            else
            {
                string errorMessage = "~r~[ERROR]~w~ That plate already exist!";
                player.TriggerEvent("BLRP_FRAMEWORK:CharacterCreationError", errorMessage);
            }
        }

        private void GiveID([FromSource] Player player, int ID, string Name, Vector3 Position)
        {
            bool useNUI = Config.Load.nuiID;
            TriggerClientEvent("BLRP_FRAMEWORK:GiveIDLocal", Name, ID, Position, useNUI);
        }

        private void GiveCCW([FromSource] Player player, int ID, string Name, Vector3 Position)
        {
            bool useNUI = Config.Load.nuiID;
            TriggerClientEvent("BLRP_FRAMEWORK:GiveCCWLocal", Name, ID, Position, useNUI);
        }

        private void CreateNewCharacter([FromSource] Player player, string firstname, string lastname, string money, string job, string license, string citations)
        {
            //Get Identifier
            var Identifier = player.Identifiers["license"];

            /* THIS IS THE OLD CODE FOR CREATING A CHARACTER THAT ONLY SUPPORTS ONE CHARACTER
            if (!DoesPlayerHaveCharacterAlready(Identifier))
            {
                //Insert to database
                Database.ExecuteInsertQuery($"INSERT INTO characters (Identifier, First_Name, Last_Name, Money, Job, License_Status, Citations) VALUES ('{Identifier}', '{firstname}', '{lastname}', '{money}', '{job}', '{license}', '{citations}')");

                //Close any database connection
                Database.Connection.Close();

                //Make string of full name
                string name = firstname + " " + lastname;

                //Trigger Event
                player.TriggerEvent("BLRP_FRAMEWORK:CharacterCreated", name);
            }
            else
            {
                string errorMessage = "~r~[ERROR]~w~ You already have created a character!";
                player.TriggerEvent("BLRP_FRAMEWORK:CharacterCreationError", errorMessage);
            }
            */


            // NEW MULTICHARACTER CODE
            if (!DoesCharacterNameAlreadyExist(firstname, lastname, Identifier))
            {
                //Insert to database
                Database.ExecuteInsertQuery($"INSERT INTO characters (Identifier, First_Name, Last_Name, Money, Job, License_Status, Citations) VALUES ('{Identifier}', '{firstname}', '{lastname}', '{money}', '{job}', '{license}', '{citations}')");

                //Close any database connection
                Database.Connection.Close();

                //Make string of full name
                string name = firstname + " " + lastname;

                //Trigger Event
                player.TriggerEvent("BLRP_FRAMEWORK:CharacterCreated", name, money, job);
            }
            else
            {
                string errorMessage = "~r~[ERROR]~w~ You already have created a character with that name!";
                player.TriggerEvent("BLRP_FRAMEWORK:CharacterCreationError", errorMessage);
            }
        }

        private void UpdateCharacter([FromSource] Player player, string firstname, string lastname, string money, string job, string license, string citations)
        {
            //Get Identifier
            var Identifier = player.Identifiers["license"];

            if (!DoesPlayerHaveCharacterAlready(Identifier))
            {
                //Insert to database
                Database.ExecuteInsertQuery($"INSERT INTO characters (Identifier, First_Name, Last_Name, Money, Job, License_Status, Citations) VALUES ('{Identifier}', '{firstname}', '{lastname}', '{money}', '{job}', '{license}', '{citations}')");

                //Close any database connection
                Database.Connection.Close();

                //Make string of full name
                string name = firstname + " " + lastname;

                //Trigger Event
                player.TriggerEvent("BLRP_FRAMEWORK:CharacterCreated", name);
            }
            else
            {
                //Update database
                Database.ExecuteUpdateQuery($"UPDATE characters SET First_Name = '{firstname}' WHERE Identifier = '{Identifier}'");
                Database.ExecuteUpdateQuery($"UPDATE characters SET Last_Name = '{lastname}' WHERE Identifier = '{Identifier}'");
                Database.ExecuteUpdateQuery($"UPDATE characters SET Money = '{money}' WHERE Identifier = '{Identifier}'");
                Database.ExecuteUpdateQuery($"UPDATE characters SET Job = '{job}' WHERE Identifier = '{Identifier}'");
                Database.ExecuteUpdateQuery($"UPDATE characters SET License_Status = '{license}' WHERE Identifier = '{Identifier}'");

                //Close any database connection
                Database.Connection.Close();

                //Trigger Event
                player.TriggerEvent("BLRP_FRAMEWORK:CharacterUpdated", firstname, lastname, money, job, license, citations);
            }
        }

        private void GetPlayerCharacters([FromSource] Player player)
        {
            //Close any connection
            Database.Connection.Close();

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Retrieve the plates
            MySqlDataReader retrievedCharacters = Database.ExecuteSelectQuery($"SELECT First_Name, Last_Name FROM characters WHERE Identifier = '{Identifier}'");
            List<dynamic> characters = new List<dynamic>();
            string first = "";
            string last = "";
            while (retrievedCharacters.Read())
            {
                first = retrievedCharacters["First_Name"].ToString();
                last = retrievedCharacters["Last_Name"].ToString();
                string fullName = first + " " + last;
                characters.Add(fullName);
            }

            //Close connection
            Database.Connection.Close();

            //Send character list to client
            player.TriggerEvent("BLRP_FRAMEWORK:GetCharacterList", characters);
        }

        private bool DoesCharacterNameAlreadyExist(string firstname, string lastname, string identifier)
        {
            MySqlDataReader Result = Database.ExecuteSelectQuery($"SELECT First_Name, Last_Name, Identifier FROM characters WHERE First_Name = '{firstname}' AND Last_Name = '{lastname}' AND Identifier = '{identifier}'");

            bool DoesCharacterNameAlreadyExist = false;
            while (Result.Read())
            {
                DoesCharacterNameAlreadyExist = true;
            }
            Database.Connection.Close();

            return DoesCharacterNameAlreadyExist;
        }
        
        private bool DoesPlayerHaveCharacterAlready(string identifier)
        {
            MySqlDataReader Result = Database.ExecuteSelectQuery($"SELECT Identifier FROM characters WHERE Identifier = '{identifier}'");

            bool DoesPlayerOwnCharacter = false;
            while (Result.Read())
            {
                DoesPlayerOwnCharacter = true;
            }
            Database.Connection.Close();

            return DoesPlayerOwnCharacter;
        }

        private bool DoesPlateAlreadyExist(string plate)
        {
            MySqlDataReader Result = Database.ExecuteSelectQuery($"SELECT Plate FROM vehicles WHERE Plate = '{plate}'");

            bool DoesPlateExist = false;
            while (Result.Read())
            {
                DoesPlateExist = true;
            }
            Database.Connection.Close();

            return DoesPlateExist;
        }
    }
}
