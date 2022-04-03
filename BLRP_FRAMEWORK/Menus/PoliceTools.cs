using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using System;

namespace BLRP_FRAMEWORK.Menus
{
    public class PoliceTools : BaseScript
    {
        private static MenuPool _menuPool;
        private static UIMenu mainMenu;

        public void VehicleTools(UIMenu menu)
        {
            var vehicletoolssub = _menuPool.AddSubMenu(menu, "Vehicle Tools");
            for (int i = 0; i < 1; i++) ;

            vehicletoolssub.MouseEdgeEnabled = false;
            vehicletoolssub.ControlDisablingEnabled = false;

            var unlockvehicle = new UIMenuItem("Unlock Vehicle", "Stand beside the car you wish to unlock.");
            vehicletoolssub.AddItem(unlockvehicle);
            vehicletoolssub.OnItemSelect += (sender, item, index) =>
            {
                if (item == unlockvehicle)
                {
                    UnlockVehicle();
                }
            };
        }

        public void MainOptions(UIMenu menu)
        {
            
        }

        public PoliceTools()
        {
            //Commands
            API.RegisterCommand("policetools", new Action(OpenMenu), false);

            //Menu Related
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Police Tools", "BlueLine Framework");
            _menuPool.Add(mainMenu);

            //Add Sub Menus
            MainOptions(mainMenu);
            VehicleTools(mainMenu);

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
            if (Main.isCop)
            {
                mainMenu.Visible = !mainMenu.Visible;
            }
            else
            {
                Screen.ShowNotification("~r~[ERROR]~w~ You are not a cop!");
            }
        }

        private static void UnlockVehicle()
        {
            if (Main.isCop)
            {
                foreach (Vehicle car in World.GetAllVehicles())
                {
                    if (World.GetDistance(Game.Player.Character.Position, car.Position) < 2f)
                    {
                        TriggerServerEvent("BLRP_FRAMEWORK:UnlockVehicle", car.Handle);
                        Screen.ShowNotification("~g~[SUCCESS]~w~ The vehicle has been unlocked");
                    }
                }
            }
        }
    }
}