using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLRP_FRAMEWORK.Menus
{
    public class ImpoundMenu : BaseScript
    {
        public static List<dynamic> impoundedPlates;
        public static List<dynamic> impoundedModels;
        public static MenuPool _menuPool;
        public static UIMenu mainMenu;

        private static string SelectedPlate = null;

        public void ImpoundedPlatesOption(UIMenu menu)
        {
            //Create List
            impoundedPlates = new List<dynamic>();
            impoundedModels = new List<dynamic>();

            //Add blank info
            impoundedPlates.Add("0");
            impoundedModels.Add("0");

            //Create menu items
            var impoundedplatesitem = new UIMenuListItem("Vehicle", impoundedPlates, 0);
            var impoundedmodelsitem = new UIMenuListItem("Vehicle", impoundedModels, 0);

            //Add Items
            menu.AddItem(impoundedmodelsitem);

            //On List Change Event
            menu.OnListChange += (sender, item, index) =>
            {
                if (item == impoundedmodelsitem)
                {
                    impoundedplatesitem.Index = impoundedmodelsitem.Index;
                    string CurrentModel = impoundedmodelsitem.Items[index].ToString();
                    string CurrentPlate = impoundedplatesitem.Items[index].ToString();
                    Screen.ShowNotification($"Current Option: ~n~~b~Model: ~y~{CurrentModel} ~n~~b~Plate: ~y~{CurrentPlate}");
                }
            };

            //On List Select Event
            menu.OnListSelect += (sender, item, index) =>
            {
                if (item == impoundedmodelsitem)
                {
                    string SelectedModel = impoundedmodelsitem.Items[index].ToString();
                    SelectedPlate = impoundedplatesitem.Items[index].ToString();
                    Screen.ShowNotification($"You have selected: ~n~~b~Model: ~y~{SelectedModel} ~n~~b~Plate: ~y~{SelectedPlate}");
                }
            };
        }

        public void MainMenuOptions(UIMenu menu)
        {
            var spawnCar = new UIMenuItem("~y~Spawn Selected Plate");
            menu.AddItem(spawnCar);
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == spawnCar)
                {
                    if (SelectedPlate == null)
                    {
                        Screen.ShowNotification("~r~[ERROR]~w~ You have not selected a plate");
                    }
                    else
                    {
                        if (SelectedPlate == "No Impounded Vehicles")
                        {
                            Screen.ShowNotification("~r~[ERROR]~w~ There are no vehicles in the impound!");
                        }
                        else
                        {
                            TriggerServerEvent("BLRP_FRAMEWORK:GetImpoundedVehicleInfo", SelectedPlate);
                        }
                    }
                }
            };
        }

        public ImpoundMenu()
        {
            //EVENTS
            EventHandlers["BLRP_SpawnImpoundedVehicle"] += new Action<string, string, string, string>(SpawnImpoundedVehicleAsync);

            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Impound Menu", "BlueLine Framework");
            _menuPool.Add(mainMenu);

            ImpoundedPlatesOption(mainMenu);
            MainMenuOptions(mainMenu);

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
            };
        }

        private static void SpawnImpoundedVehicleAsync(string model, string primarycolor, string secondarycolor, string plate)
        {
            SelectedPlate = null;
            SpawnVehicle(model, plate, primarycolor, secondarycolor);
        }

        private static async Task SpawnVehicle(string model, string plate, string primaryColor, string secondaryColor)
        {
            Vector3 SpawnLocation = new Vector3(446.45935058594f, -1019.2201538086f, 27.861753463745f);
            Vehicle car = await World.CreateVehicle(model, SpawnLocation, 91.52f);
            car.Mods.LicensePlate = plate;
            API.SetVehicleModKit(car.Handle, 0);
            API.SetVehicleColours(car.Handle, int.Parse(primaryColor), int.Parse(secondaryColor));
            API.TaskWarpPedIntoVehicle(Game.Player.Character.Handle, car.Handle, -1);
            mainMenu.Visible = !mainMenu.Visible;
        }
    }
}