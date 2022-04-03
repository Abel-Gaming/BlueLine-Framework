using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using System;
using System.Collections.Generic;

namespace BLRP_FRAMEWORK.Menus
{
    public class SwitchCharacters : BaseScript
    {
        private static MenuPool _menuPool;
        private static UIMenu mainMenu;
        private static List<dynamic> MyCharacters;

        public void AddMenuOptions(UIMenu menu)
        {
            MyCharacters = new List<dynamic>();
            MyCharacters.Add("null");

            var selectCharacter = new UIMenuListItem("Select Character", MyCharacters, 0);
            menu.AddItem(selectCharacter);
            menu.OnListSelect += (sender, item, index) =>
            {
                if (item == selectCharacter)
                {
                    string selectedCharacter = selectCharacter.Items[index].ToString();
                    TriggerServerEvent("BLRP_FRAMEWORK:GetSelectedCharacterInfo", selectedCharacter);
                }
            };
        }

        public SwitchCharacters()
        {
            //Register Commands
            API.RegisterCommand("switch-character", new Action(OpenMenu), false);

            //Events
            EventHandlers["BLRP_FRAMEWORK:GetCharacterList"] += new Action<List<dynamic>>(GetCharacterList);
            EventHandlers["BLRP_FRAMEWORK:GetCharacterInfoReturn"] += new Action<string, string, int, string>(SetCharacterInfo);

            //Menu Items
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Switch Characters", "BlueLine Framework");
            _menuPool.Add(mainMenu);

            //Add Menu Options
            AddMenuOptions(mainMenu);

            //Menu Pool
            _menuPool.MouseEdgeEnabled = false;
            _menuPool.ControlDisablingEnabled = false;
            _menuPool.RefreshIndex();

            //Tick
            Tick += async () =>
            {
                _menuPool.ProcessMenus();
            };
        }

        private static void OpenMenu()
        {
            TriggerServerEvent("BLRP_FRAMEWORK:GetPlayersCharacters");
        }

        private static void SetCharacterInfo(string firstName, string lastName, int money, string job)
        {
            //Check to see if this character is a cop
            if (job == "police")
            {
                Main.isCop = true;
            }
            else
            {
                Main.isCop = false;
            }
            CharacterSystem.CharacterMain.Name = $"{firstName} {lastName}"; //Set character name
            CharacterSystem.CharacterMain.Money = money; //Set character money
            CharacterSystem.CharacterMain.Job = job; //Set character job
            Screen.ShowNotification($"~g~[SUCCESS]~w~ You have switched to character ~b~{CharacterSystem.CharacterMain.Name}"); //Show notification
        }

        private static void GetCharacterList(List<dynamic> Characters)
        {
            MyCharacters.Clear();
            foreach (var character in Characters)
            {
                MyCharacters.Add(character);
            }
            mainMenu.Visible = !mainMenu.Visible;
        }
    }
}