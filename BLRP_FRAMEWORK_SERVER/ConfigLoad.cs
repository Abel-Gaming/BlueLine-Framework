using CitizenFX.Core;
using SharpConfig;
using System;
using static CitizenFX.Core.Native.API;

namespace BLRP_FRAMEWORK_SERVER
{
    public class Config
    {
        public static Load Load;
    }

    public struct Load
    {
        public string Host;
        public uint Port;
        public string User;
        public string Password;
        public string Database;

        public bool nuiID;

        public bool UltrunzBCES;
        public bool GabzMRPD;

        public static void Config()
        {
            Load config = new Load()
            {
                //MYSQL
                Host = "127.0.0.1",
                Port = 3306,
                User = "root",
                Password = "",
                Database = "blrp",

                //Settings
                nuiID = true,

                //Addons
                UltrunzBCES = false,
                GabzMRPD = false
            };

            var configFile = Configuration.LoadFromFile(string.Format("{0}/server/config/config.ini", GetResourcePath(GetCurrentResourceName())));

            try
            {
                bool isFileNotNull = !string.IsNullOrWhiteSpace(configFile.ToString());

                if (isFileNotNull)
                {
                    //Get MQSQL settings section
                    var mysqlConfigsection = configFile["MYSQL"];

                    config.Host = mysqlConfigsection["HOST"].StringValue;
                    config.Port = (uint)mysqlConfigsection["PORT"].IntValue;
                    config.User = mysqlConfigsection["USER"].StringValue;
                    config.Database = mysqlConfigsection["DATABASE"].StringValue;

                    //Get server settings section
                    var settingsConfigsection = configFile["SETTINGS"];

                    config.nuiID = settingsConfigsection["NUIID"].BoolValue;

                    //Get addons settings section
                    var addonsConfigsection = configFile["ADDONS"];

                    config.UltrunzBCES = addonsConfigsection["UltrunzBCES"].BoolValue;
                    config.GabzMRPD = addonsConfigsection["GabzMRPD"].BoolValue;

                    //Set the config
                    BLRP_FRAMEWORK_SERVER.Config.Load = config;

                    //Write to the log
                    Debug.WriteLine("[SUCCESS] The configuration file has been loaded!");

                    //Initialize the database
                    BLRP_FRAMEWORK_SERVER.Database.Initialize();
                }
                else
                {
                    //Empty config error
                    Debug.WriteLine("[ERROR] The configuration file is empty. Please make sure the information is in there!");
                    return;
                }
            }
            catch (Exception ex)
            {
                //Error with config
                Debug.WriteLine($"[ERROR] Something went wrong while loading the config. {ex}");
                return;
            }
        }
    }
}