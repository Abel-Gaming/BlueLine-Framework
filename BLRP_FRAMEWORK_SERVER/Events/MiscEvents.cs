using System;
using CitizenFX.Core;
using System.Collections.Generic;

namespace BLRP_FRAMEWORK_SERVER.Events
{
    public class MiscEvents : BaseScript
    {
        public MiscEvents()
        {
            EventHandlers["BLRP_FRAMEWORK:GetPlayerList"] += new Action<Player>(GetAllPlayers);
            EventHandlers["BLRP_FRAMEWORK:SetVehicleSoundServer"] += new Action<int, string>(SetVehicleSoundServer);
        }

        private void SetVehicleSoundServer(int vehicle, string sound)
        {
            TriggerClientEvent("BLRP_FRAMEWROK:SetVehicleSoundClient", vehicle, sound);
        }

        private static void GetAllPlayers([FromSource] Player player)
        {
            PlayerList players = new PlayerList();
            List<dynamic> playerList = new List<dynamic>();
            List<dynamic> playerNameList = new List<dynamic>();
            foreach (Player p in players)
            {
                playerList.Add(p.Handle);
                playerNameList.Add(p.Name);
            }
            player.TriggerEvent("BLRP_FRAMEWORK:SendPlayerList", playerList, playerNameList);
        }
    }
}
