using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using System;
using System.Collections.Generic;

namespace BLRP_FRAMEWORK.Menus
{
    public class AutoLock : BaseScript
    {
        // Auto Lock References //
        private static Vehicle PatrolCar = null;
        private static bool VehicleLocked = false;
        private static bool AutoLockVehicle = false;
        private static bool LeaveEngineOn = false;
        private static Double VehicleDistance = 7.5;

        // Menu References //
        private MenuPool _menuPool;
        public static UIMenu mainMenu;
        public static TextTimerBar unlockTimeBar;

        public void MainMenuOptions(UIMenu menu)
        {
            var SaveVehicle = new UIMenuItem("Set Patrol Vehicle");
            menu.AddItem(SaveVehicle);
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == SaveVehicle)
                {
                    if (Game.Player.Character.IsInVehicle())
                    {
                        if (Game.Player.Character.CurrentVehicle.ClassType == VehicleClass.Emergency)
                        {
                            PatrolCar = Game.Player.Character.CurrentVehicle;
                            VehicleLocked = false;
                            Screen.ShowNotification("~g~[SUCCESS]~w~ Patrol vehicle set");
                        }
                        else
                        {
                            Screen.ShowNotification("~r~[ERROR]~w~ You are not in an emergency vehicle");
                        }
                    }
                    else
                    {
                        Screen.ShowNotification("~r~[ERROR]~w~ You are not in a vehicle");
                    }
                }
            };

            var LeaveEngineRunning = new UIMenuCheckboxItem("Leave Engine Running", LeaveEngineOn, "This will leave your vehicle running when you exit");
            menu.AddItem(LeaveEngineRunning);
            menu.OnCheckboxChange += (sender, item, index) =>
            {
                if (item == LeaveEngineRunning)
                {
                    LeaveEngineOn = !LeaveEngineOn;
                }
            };

            var AutoLockCar = new UIMenuCheckboxItem("Auto Lock", AutoLockVehicle, "This will automatically lock your patrol vehicle after a set distance");
            menu.AddItem(AutoLockCar);
            menu.OnCheckboxChange += (sender, item, index) =>
            {
                if (item == AutoLockCar)
                {
                    AutoLockVehicle = !AutoLockVehicle;
                }
            };

            var LockCar = new UIMenuItem("Lock Car");
            menu.AddItem(LockCar);
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == LockCar)
                {
                    VehicleLocked = !VehicleLocked;
                    Screen.ShowNotification("~y~[WARNING]~w~ Your patrol vehicle has been locked");
                    TriggerServerEvent("BLRP_FRAMEWORK:LockVehicle", PatrolCar.Handle);
                }
            };

            var UnlockCar = new UIMenuItem("Unlock Car");
            menu.AddItem(UnlockCar);
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == UnlockCar)
                {
                    VehicleLocked = !VehicleLocked;
                    Screen.ShowNotification("~g~[SUCCESS]~w~ Your patrol vehicle has been unlocked");
                    TriggerServerEvent("BLRP_FRAMEWORK:UnlockVehicle", PatrolCar.Handle);
                }
            };
        }

        public void AutoLockSettings(UIMenu menu)
        {
            var AutoLockSettingsSub = _menuPool.AddSubMenu(menu, "~y~Auto Lock Settings");
            for (int i = 0; i < 1; i++) ;

            AutoLockSettingsSub.MouseEdgeEnabled = false;
            AutoLockSettingsSub.ControlDisablingEnabled = false;

            var distances = new List<dynamic>
            {
                "3", "5", "7.5", "10"
            };
            var SelectDistance = new UIMenuListItem("Select Distance", distances, 0);
            AutoLockSettingsSub.AddItem(SelectDistance);
            AutoLockSettingsSub.OnListSelect += (sender, item, index) =>
            {
                if (item == SelectDistance)
                {
                    if (SelectDistance.Items[index].ToString() == "3")
                    {
                        VehicleDistance = 3.0;
                        Screen.ShowSubtitle("Distance Set to: ~b~" + VehicleDistance.ToString());
                    }
                    else if (SelectDistance.Items[index].ToString() == "5")
                    {
                        VehicleDistance = 5.0;
                        Screen.ShowSubtitle("Distance Set to: ~b~" + VehicleDistance.ToString());
                    }
                    else if (SelectDistance.Items[index].ToString() == "7.5")
                    {
                        VehicleDistance = 7.5;
                        Screen.ShowSubtitle("Distance Set to: ~b~" + VehicleDistance.ToString());
                    }
                    else if (SelectDistance.Items[index].ToString() == "10")
                    {
                        VehicleDistance = 10.0;
                        Screen.ShowSubtitle("Distance Set to: ~b~" + VehicleDistance.ToString());
                    }
                }
            };
        }

        public void VehicleAudioSettings(UIMenu menu)
        {
            var AutoLockSettingsSub = _menuPool.AddSubMenu(menu, "~y~Vehicle Engine Settings");
            for (int i = 0; i < 1; i++) ;

            AutoLockSettingsSub.MouseEdgeEnabled = false;
            AutoLockSettingsSub.ControlDisablingEnabled = false;

            var distances = new List<dynamic>
            {
                "Police", "Charger (1)", "Charger (2)", "FPIU/FPIS", "CVPI" ,"Ram" ,"Mustang", "r34", "Hellcat"
            };
            var SelectDistance = new UIMenuListItem("Select Engine Sound", distances, 0);
            AutoLockSettingsSub.AddItem(SelectDistance);
            AutoLockSettingsSub.OnListSelect += (sender, item, index) =>
            {
                if (item == SelectDistance)
                {
                    if (SelectDistance.Items[index].ToString() == "Police")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "POLICE";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~police");
                    }
                    else if (SelectDistance.Items[index].ToString() == "Charger (1)")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "WINDSOR";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~Charger (1)");
                    }
                    else if (SelectDistance.Items[index].ToString() == "Charger (2)")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "npolchar";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~Charger (2)");
                    }
                    else if (SelectDistance.Items[index].ToString() == "Mustang")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "fordvoodoo";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~Mustang");
                    }
                    else if (SelectDistance.Items[index].ToString() == "r34")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "r34sound";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~r34");
                    }
                    else if (SelectDistance.Items[index].ToString() == "Hellcat")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "dodgehemihellcat";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~Dodge Hellcat");
                    }
                    else if (SelectDistance.Items[index].ToString() == "FPIU/FPIS")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "ecoboostv6";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~FPIU/FPIS");
                    }
                    else if (SelectDistance.Items[index].ToString() == "CVPI")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "cvpiv8";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~CVPI");
                    }
                    else if (SelectDistance.Items[index].ToString() == "Ram")
                    {
                        int currentVehicle = Game.Player.Character.CurrentVehicle.Handle;
                        string sound = "cummins5924v";
                        TriggerServerEvent("BLRP_FRAMEWORK:SetVehicleSoundServer", currentVehicle, sound);
                        Screen.ShowSubtitle("Vehicle audio has been set to ~b~Ram");
                    }
                }
            };
        }

        public AutoLock()
        {
            //Events
            EventHandlers["BLRP_FRAMEWORK:LockVehicleClient"] += new Action<int>(LockVehicleFunction);
            EventHandlers["BLRP_FRAMEWORK:UnlockVehicleClient"] += new Action<int>(UnlockVehicleFunction);
            EventHandlers["BLRP_FRAMEWROK:SetVehicleSoundClient"] += new Action<int, string>(SetEngineSound);

            //Commands
            API.RegisterCommand("+autolockmenu", new Action(OpenMenuCommand), false);
            API.RegisterCommand("-autolockmenu", new Action(NoFunctionCommand), false);
            API.RegisterCommand("+unlockpatrolcar", new Action(UnlockPatrolCar), false);
            API.RegisterCommand("-unlockpatrolcar", new Action(NoFunctionCommand), false);

            //Key Mappings
            API.RegisterKeyMapping("+autolockmenu", "Open Auto Lock Menu", "KEYBOARD", "F3");
            API.RegisterKeyMapping("+unlockpatrolcar", "Unlock Patrol Car", "KEYBOARD", "`");

            //Menu Stuff
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Patrol Car Menu", "BlueLine Framework");
            _menuPool.Add(mainMenu);

            MainMenuOptions(mainMenu);
            AutoLockSettings(mainMenu);
            VehicleAudioSettings(mainMenu);

            _menuPool.MouseEdgeEnabled = false;
            _menuPool.ControlDisablingEnabled = false;
            _menuPool.RefreshIndex();

            Tick += async () =>
            {
                _menuPool.ProcessMenus();

                if (_menuPool.IsAnyMenuOpen())
                {
                    Game.DisableControlThisFrame(0, Control.MeleeAttackLight);
                    Game.DisableControlThisFrame(0, Control.MeleeAttack1);
                }
                else
                {
                    Game.EnableControlThisFrame(0, Control.MeleeAttackLight);
                    Game.EnableControlThisFrame(0, Control.MeleeAttack1);
                }

                //Auto Lock Tick
                if (PatrolCar == null)
                {

                }
                else
                {
                    if (AutoLockVehicle)
                    {
                        Vector3 playerCoords = Game.Player.Character.Position;
                        Vector3 vehicleCoords = PatrolCar.Position;
                        if (!VehicleLocked & API.GetDistanceBetweenCoords(playerCoords.X, playerCoords.Y, playerCoords.Z, vehicleCoords.X, vehicleCoords.Y, vehicleCoords.Z, false) >= VehicleDistance)
                        {
                            TriggerServerEvent("BLRP_FRAMEWORK:LockVehicle", PatrolCar.Handle);
                            Screen.ShowNotification("~y~[WARNING]~w~ Your patrol vehicle has been auto-locked");
                            VehicleLocked = true;
                        }
                    }
                }

                //Engine On Tick
                if (PatrolCar == null)
                {

                }
                else
                {
                    if (LeaveEngineOn)
                    {
                        if (!Game.Player.Character.IsInVehicle())
                        {
                            PatrolCar.IsEngineRunning = true;
                        }
                    }
                }
            };
        }

        private static void NoFunctionCommand()
        {

        }

        private static void UnlockPatrolCar()
        {
            if (VehicleLocked)
            {
                if (PatrolCar == null)
                {

                }
                else
                {
                    VehicleLocked = !VehicleLocked;
                    Screen.ShowNotification("~g~[SUCCESS]~w~ Your patrol vehicle has been unlocked");
                    TriggerServerEvent("BLRP_FRAMEWORK:UnlockVehicle", PatrolCar.Handle);
                }
            }
        }

        private void LockVehicleFunction(int CarToLock)
        {
            API.SetVehicleDoorsLocked(CarToLock, 2);
        }

        private void UnlockVehicleFunction(int CarToUnlock)
        {
            API.SetVehicleDoorsLocked(CarToUnlock, 1);
        }

        private void SetEngineSound(int car, string sound)
        {
            API.ForceVehicleEngineAudio(car, sound);
        }

        private static void OpenMenuCommand()
        {
            mainMenu.Visible = !mainMenu.Visible;
        }
    }
}