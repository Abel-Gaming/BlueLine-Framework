using System;
using System.Collections.Generic;
using MySqlConnector;
using CitizenFX.Core;

namespace BLRP_FRAMEWORK_SERVER.Events
{
    public class PoliceEvents : BaseScript
    {
        public PoliceEvents()
        {
            //Config Events
            EventHandlers["BLRP_FRAMEWORK:GetAddonConfig"] += new Action<Player>(GetAddonsConfig);

            //Impound Events
            EventHandlers["BLRP_FRAMEWORK:ImpoundVehicle"] += new Action<Player, string, string, string, string>(ImpoundCar);
            EventHandlers["BLRP_FRAMEWORK:GetImpoundedVehicles"] += new Action<Player>(GetImpoundedVehicles);
            EventHandlers["BLRP_FRAMEWORK:GetImpoundedVehicleInfo"] += new Action<Player, string>(GetImpoundedVehicleInfo);

            //Ticket Events
            EventHandlers["BLRP_FRAMEWORK:IssueTicket"] += new Action<Player, int, string, string>(IssueTicket);

            //Duty Events
            EventHandlers["BLRP_FRAMEWORK:PoliceOnDuty"] += new Action<Player>(PoliceOnDuty);
            EventHandlers["BLRP_FRAMEWORK:PoliceOffDuty"] += new Action<Player>(PoliceOffDuty);

            //Vehicle Events
            EventHandlers["BLRP_FRAMEWORK:UnlockVehicle"] += new Action<Player, int>(UnlockVehicle);
            EventHandlers["BLRP_FRAMEWORK:LockVehicle"] += new Action<Player, int>(LockVehicle);

            //External
            EventHandlers["BLRP_FRAMEWORK:RequestNameSearch"] += new Action<Player, string, string>(RequestNameSearch);
            EventHandlers["BLRP_FRAMEWORK:RequestPlateSearch"] += new Action<Player, string>(RequestPlateSearch);
        }

        private void GetAddonsConfig([FromSource] Player player)
        {
            bool UltrunzBCES = Config.Load.UltrunzBCES;
            bool GabzMRPD = Config.Load.GabzMRPD;
            player.TriggerEvent("BLRP_FRAMEWORK:ReturnAddonConfig", UltrunzBCES, GabzMRPD);
        }

        private void ImpoundCar([FromSource] Player player, string plate, string model, string primarycolor, string secondarycolor)
        {
            // Get player
            string playerName = player.Name;

            // Check to see if the car is already impounded
            if (!string.IsNullOrEmpty(plate))
            {
                if (!IsVehicleAlreadyImpounded(plate))
                {
                    // If the car is not already impounded, impound it
                    Database.ExecuteInsertQuery($"INSERT INTO impound (Plate, Model, Impounded_By, Primary_Color, Secondary_Color) VALUES ('{plate}', '{model}', '{playerName}', '{primarycolor}', '{secondarycolor}')");
                    player.TriggerEvent("BLRP_FRAMEWORK:ImpoundNotification", plate, model);
                    
                    //Close any database connection
                    Database.Connection.Close();
                }
                else
                {
                    // If the car is already impounded, notify the player
                    player.TriggerEvent("BLRP_FRAMEWORK:AlreadyImpoundedNotification", plate, model);
                }
            }
        }

        private void IssueTicket([FromSource] Player player, int recipientID, string amount, string violation)
        {
            //Close any database connection
            Database.Connection.Close();

            //Generate playerlist
            PlayerList Playerlist = new PlayerList();

            //Get recipient
            Player recipient = Playerlist[recipientID];
            string recipientName = recipient.Name;

            //Get Recipient identifier
            var Identifier = recipient.Identifiers["license"];

            //Get how many tickets the player has
            MySqlDataReader retrievedjob = Database.ExecuteSelectQuery($"SELECT Citations FROM characters WHERE Identifier = '{Identifier}'");
            string citationString = "";
            while (retrievedjob.Read())
            {
                citationString = retrievedjob["Citations"].ToString();
            }

            //Convert to int
            int citations = int.Parse(citationString);

            //Increase the citation count
            citations = citations + 1;

            //Close connection
            Database.Connection.Close();

            //Update the citation count
            Database.ExecuteUpdateQuery($"UPDATE characters SET Citations = '{citations.ToString()}' WHERE Identifier = '{Identifier}'");

            //Close Connection
            Database.Connection.Close();

            //Get officer name
            string officer = player.Name;

            //Get date as string
            string issued = DateTime.Now.ToString();

            //Insert to database
            Database.ExecuteInsertQuery($"INSERT INTO tickets (RecipientID, Recipient, Officer, Amount, Violation, Issued) VALUES ('{Identifier}', '{recipientName}', '{officer}', '{amount}', '{violation}', '{issued}')");

            //Close any database connection
            Database.Connection.Close();

            //Alert the player (cop) they issued a citation
            player.TriggerEvent("BLRP_FRAMEWORK:TicketIssuedNotification", recipientName, amount, violation);

            //Alert the recipient they recieved a ticket
            recipient.TriggerEvent("BLRP_FRAMEWORK:TicketRecievedNotification", officer, amount, violation);
        }

        private void PoliceOnDuty([FromSource] Player player)
        {
            //Close any database connection
            Database.Connection.Close();

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Update database
            Database.ExecuteUpdateQuery($"UPDATE users SET Job = 'police' WHERE Identifier = '{Identifier}'");
            Database.ExecuteUpdateQuery($"UPDATE characters SET Job = 'police' WHERE Identifier = '{Identifier}'");

            //Close Connection
            Database.Connection.Close();

            //Trigger client job notification event
            player.TriggerEvent("BLRP_FRAMEWORK:UpdatedJobNotification", "police");

            //Tell the console what happened
            string name = player.Name;
            Debug.WriteLine($"{name} is now on duty as a police officer");
        }

        private void PoliceOffDuty([FromSource] Player player)
        {
            //Close any database connection
            Database.Connection.Close();

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Update database
            Database.ExecuteUpdateQuery($"UPDATE users SET Job = 'unemployed' WHERE Identifier = '{Identifier}'");
            Database.ExecuteUpdateQuery($"UPDATE characters SET Job = 'unemployed' WHERE Identifier = '{Identifier}'");

            //Close Connection
            Database.Connection.Close();

            //Trigger client job notification event
            player.TriggerEvent("BLRP_FRAMEWORK:UpdatedJobNotification", "unemployed");

            //Tell the console what happened
            string name = player.Name;
            Debug.WriteLine($"{name} is now off duty");
        }

        private void GetImpoundedVehicles([FromSource] Player player)
        {
            //Close any connection
            Database.Connection.Close();

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Retrieve the plates
            MySqlDataReader retrievedplates = Database.ExecuteSelectQuery($"SELECT Plate FROM impound");
            List<dynamic> plates = new List<dynamic>();
            while (retrievedplates.Read())
            {
                plates.Add(retrievedplates["Plate"].ToString());
            }

            //Close connection
            Database.Connection.Close();

            //Retrieve plate models
            MySqlDataReader retrievedmodels = Database.ExecuteSelectQuery($"SELECT Model FROM impound");
            List<dynamic> plateModels = new List<dynamic>();
            while (retrievedmodels.Read())
            {
                plateModels.Add(retrievedmodels["Model"].ToString());
            }

            //Close connection
            Database.Connection.Close();

            //Send plate list to client
            player.TriggerEvent("BLRP_FRAMEWORK:GetImpoundedPlateList", plates, plateModels);
        }

        private void GetImpoundedVehicleInfo([FromSource] Player player, string plate)
        {
            //Close any connection
            Database.Connection.Close();

            //Get identifier
            var Identifier = player.Identifiers["license"];

            //Retrieve the model
            MySqlDataReader selectModel = Database.ExecuteSelectQuery($"SELECT Model FROM impound WHERE Plate = '{plate}'");
            string model = "";
            while (selectModel.Read())
            {
                model = selectModel["Model"].ToString();
            }

            //Close connection
            Database.Connection.Close();

            //Retrieve the primary color
            MySqlDataReader selectPrimary = Database.ExecuteSelectQuery($"SELECT Primary_Color FROM impound WHERE Plate = '{plate}'");
            string primaryColor = "";
            while (selectPrimary.Read())
            {
                primaryColor = selectPrimary["Primary_Color"].ToString();
            }

            //Close connection
            Database.Connection.Close();

            //Retrieve the secondary color
            MySqlDataReader selectSecondary = Database.ExecuteSelectQuery($"SELECT Secondary_Color FROM impound WHERE Plate = '{plate}'");
            string secondaryColor = "";
            while (selectSecondary.Read())
            {
                secondaryColor = selectSecondary["Secondary_Color"].ToString();
            }

            //Close connection
            Database.Connection.Close();

            //Delete the plate from the database
            Database.ExecuteUpdateQuery($"DELETE FROM impound WHERE Plate = '{plate}'");

            //Close connection
            Database.Connection.Close();

            //Trigger Client Event
            player.TriggerEvent("BLRP_SpawnImpoundedVehicle", model, primaryColor, secondaryColor, plate);
        }

        private void UnlockVehicle([FromSource] Player player, int vehicleID)
        {
            TriggerClientEvent("BLRP_FRAMEWORK:UnlockVehicleClient", vehicleID);
        }

        private void LockVehicle([FromSource] Player player, int vehicleID)
        {
            TriggerClientEvent("BLRP_FRAMEWORK:LockVehicleClient", vehicleID);
        }

        private void RequestNameSearch([FromSource] Player player, string first, string last)
        {
            if (DoesPedExist(first, last))
            {
                //Close any connection
                Database.Connection.Close();

                //Get identifier
                var Identifier = player.Identifiers["license"];

                //Retrieve info
                MySqlDataReader retrievedjob = Database.ExecuteSelectQuery($"SELECT Job, License_Status, Citations FROM characters WHERE First_Name = '{first}' AND Last_Name = '{last}'");
                string job = "";
                string license = "";
                string citations = "";
                while (retrievedjob.Read())
                {
                    job = retrievedjob["Job"].ToString();
                    license = retrievedjob["License_Status"].ToString();
                    citations = retrievedjob["Citations"].ToString();
                }

                //Close connection
                Database.Connection.Close();

                //Trigger the event back
                player.TriggerEvent("BLFW_MDT:GetNameResults", first, last, job, license, citations);
            }
            else
            {
                string error = "That person does not exist";
                player.TriggerEvent("BLRP_FRAMEWORK:ErrorMessage", error);
                player.TriggerEvent("BLFW_MDT:NoPedResult");
            }
        }

        private void RequestPlateSearch([FromSource] Player player, string plate)
        {
            if (DoesPlateExist(plate))
            {
                //Close any connection
                Database.Connection.Close();

                //Get identifier
                var Identifier = player.Identifiers["license"];

                //Retrieve info
                MySqlDataReader retrievedjob = Database.ExecuteSelectQuery($"SELECT Model_Name, Class_Type, Registered_To FROM vehicles WHERE Plate = '{plate}'");
                string model = "";
                string classType = "";
                string owner = "";
                while (retrievedjob.Read())
                {
                    model = retrievedjob["Model_Name"].ToString();
                    classType = retrievedjob["Class_Type"].ToString();
                    owner = retrievedjob["Registered_To"].ToString();
                }

                //Close connection
                Database.Connection.Close();

                //Trigger the event back
                player.TriggerEvent("BLFW_MDT:GetPlateResults", model, classType, owner, plate);
            }
            else
            {
                string error = "That plate does not exist";
                player.TriggerEvent("BLRP_FRAMEWORK:ErrorMessage", error);
                player.TriggerEvent("BLFW_MDT:NoPlateResult");
            }
        }

        private bool IsVehicleAlreadyImpounded(string plate)
        {
            MySqlDataReader Result = Database.ExecuteSelectQuery($"SELECT Plate FROM impound WHERE Plate = '{plate}'");

            bool IsVehicleAlreadyImpounded = false;
            while (Result.Read())
            {
                IsVehicleAlreadyImpounded = true;
            }
            Database.Connection.Close();

            return IsVehicleAlreadyImpounded;
        }

        private bool DoesPlateExist(string plate)
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

        private bool DoesPedExist(string first, string last)
        {
            MySqlDataReader Result = Database.ExecuteSelectQuery($"SELECT First_Name, Last_Name FROM characters WHERE First_Name = '{first}' AND Last_Name = '{last}'");

            bool DoesPedExist = false;
            while (Result.Read())
            {
                DoesPedExist = true;
            }
            Database.Connection.Close();

            return DoesPedExist;
        }
    }
}
