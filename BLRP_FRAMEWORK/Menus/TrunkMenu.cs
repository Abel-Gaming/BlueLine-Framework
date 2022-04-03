using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;

namespace BLRP_FRAMEWORK.Menus
{
    public class TrunkMenu : BaseScript
    {
        //Trunk Items
        private bool RifleRacked = true;
        private bool ShotgunRacked = true;
        private bool hasShotgun = false;
        private bool hasRifle = false;

        //Menu Items
        private MenuPool _menuPool;
        private UIMenu mainMenu;

        public void MainOptions(UIMenu menu)
        {
            var shotgun = new UIMenuItem("Equip Shotgun");
            shotgun.SetRightBadge(UIMenuItem.BadgeStyle.Gun);
            menu.AddItem(shotgun);
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == shotgun)
                {
                    if (hasShotgun)
                    {
                        ShotgunRacked = true;
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Screen.ShowNotification("~y~[NOTICE] ~w~Shotgun Stored");
                        Game.Player.Character.Weapons.Remove(WeaponHash.PumpShotgun);
                        shotgun.Text = "Equip Shotgun";
                        hasShotgun = false;
                    }
                    else
                    {
                        ShotgunRacked = false;
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Screen.ShowNotification("~g~[SUCCESS] ~w~Shotgun Equipped");
                        Game.Player.Character.Weapons.Give(WeaponHash.PumpShotgun, 100, true, true);
                        shotgun.Text = "Store Shotgun";
                        hasShotgun = true;
                    }
                }
            };

            var rifle = new UIMenuItem("Equip Rifle");
            rifle.SetRightBadge(UIMenuItem.BadgeStyle.Gun);
            menu.AddItem(rifle);
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == rifle)
                {
                    if (hasRifle)
                    {
                        RifleRacked = true;
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Screen.ShowNotification("~y~[NOTICE] ~w~Rifle Stored");
                        Game.Player.Character.Weapons.Remove(WeaponHash.CarbineRifle);
                        rifle.Text = "Equip Rifle";
                        hasRifle = false;
                    }
                    else
                    {
                        RifleRacked = false;
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Screen.ShowNotification("~g~[SUCCESS] ~w~Rifle Equipped");
                        Game.Player.Character.Weapons.Give(WeaponHash.CarbineRifle, 100, true, true);
                        rifle.Text = "Store Rifle";
                        hasRifle = true;
                    }
                }
            };

            var armor = new UIMenuItem("Equip Body Armor");
            armor.SetRightBadge(UIMenuItem.BadgeStyle.Armour);
            menu.AddItem(armor);
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == armor)
                {
                    Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                    await BaseScript.Delay(3000);
                    Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                    await BaseScript.Delay(1500);
                    Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                    await BaseScript.Delay(3000);
                    Game.Player.Character.Armor = 100;
                    Screen.ShowNotification("~g~[SUCCESS] ~w~Body Armor Equipped");
                }
            };
        }

        public void MainOptions2(UIMenu menu)
        {
            var CloseTrunk = new UIMenuItem("~r~Close Trunk");
            menu.AddItem(CloseTrunk);
            menu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == CloseTrunk)
                {
                    if (Game.Player.Character.LastVehicle == null)
                    {

                    }
                    else
                    {
                        if (_menuPool.IsAnyMenuOpen())
                        {
                            _menuPool.CloseAllMenus();
                        }
                        Game.Player.Character.LastVehicle.Doors[VehicleDoorIndex.Trunk].Close();
                    }
                }
            };
        }

        public void AmmoMenu(UIMenu menu)
        {
            var ammosubmenu = _menuPool.AddSubMenu(menu, "Refill Ammo");
            for (int i = 0; i < 1; i++) ;

            ammosubmenu.MouseEdgeEnabled = false;
            ammosubmenu.ControlDisablingEnabled = false;

            var stungun = new UIMenuItem("Stun Gun");
            stungun.SetRightBadge(UIMenuItem.BadgeStyle.Ammo);
            ammosubmenu.AddItem(stungun);
            ammosubmenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == stungun)
                {
                    if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.StunGun))
                    {
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Weapons[WeaponHash.StunGun].Ammo = 5;
                    }
                }
            };


            var pistol = new UIMenuItem("Pistol Ammo", "Pistol & Combat Pistol Supported");
            pistol.SetRightBadge(UIMenuItem.BadgeStyle.Ammo);
            ammosubmenu.AddItem(pistol);
            ammosubmenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == pistol)
                {
                    if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.Pistol))
                    {
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Weapons[WeaponHash.Pistol].Ammo = 100;
                    }
                    if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.CombatPistol))
                    {
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Weapons[WeaponHash.CombatPistol].Ammo = 100;
                    }
                }
            };

            var shotgun = new UIMenuItem("Shotgun");
            shotgun.SetRightBadge(UIMenuItem.BadgeStyle.Ammo);
            ammosubmenu.AddItem(shotgun);
            ammosubmenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == shotgun)
                {
                    if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.PumpShotgun))
                    {
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Weapons[WeaponHash.PumpShotgun].Ammo = 100;
                    }
                }
            };

            var rifle = new UIMenuItem("Carbine Rifle");
            rifle.SetRightBadge(UIMenuItem.BadgeStyle.Ammo);
            ammosubmenu.AddItem(rifle);
            ammosubmenu.OnItemSelect += async (sender, item, index) =>
            {
                if (item == rifle)
                {
                    if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.CarbineRifle))
                    {
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@base", "base");
                        await BaseScript.Delay(1500);
                        Game.Player.Character.Task.PlayAnimation("amb@prop_human_bum_bin@exit", "exit");
                        await BaseScript.Delay(3000);
                        Game.Player.Character.Weapons[WeaponHash.CarbineRifle].Ammo = 100;
                    }
                }
            };
        }

        public TrunkMenu()
        {
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Trunk Menu", "BlueLine Framework");
            _menuPool.Add(mainMenu);

            MainOptions(mainMenu);
            AmmoMenu(mainMenu);
            MainOptions2(mainMenu);

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

                //Weapon Check
                if (Main.isCop)
                {
                    if (RifleRacked)
                    {
                        if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.CarbineRifle))
                        {
                            Game.Player.Character.Weapons.Remove(WeaponHash.CarbineRifle);
                        }
                    }

                    if (ShotgunRacked)
                    {
                        if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.PumpShotgun))
                        {
                            Game.Player.Character.Weapons.Remove(WeaponHash.PumpShotgun);
                        }
                    }
                }

                //Trunk Check
                var CurrentVehicle = Game.Player.Character.LastVehicle;
                if (CurrentVehicle == null)
                {
                    //Do nothing
                }
                else
                {
                    Vector3 playerPosition = Game.Player.Character.Position;
                    Vector3 carPosition = CurrentVehicle.Position;
                    Vector3 carBonePosition = CurrentVehicle.Bones["boot"].Position;
                    if (CurrentVehicle.ClassType == VehicleClass.Emergency & API.GetDistanceBetweenCoords(playerPosition.X, playerPosition.Y, playerPosition.Z, carBonePosition.X, carBonePosition.Y, carBonePosition.Z, false) <= 1.75)
                    {
                        if (!Game.Player.Character.IsInVehicle())
                        {
                            Screen.DisplayHelpTextThisFrame("Press ~INPUT_VEH_FLY_SELECT_PREV_WEAPON~ to open trunk");
                            if (API.IsControlJustPressed(0, 116))
                            {
                                mainMenu.Visible = true;
                                CurrentVehicle.Doors[VehicleDoorIndex.Trunk].Open();
                            }

                        }
                    }
                    else
                    {
                        if (mainMenu.Visible)
                        {
                            mainMenu.Visible = false;
                            CurrentVehicle.Doors[VehicleDoorIndex.Trunk].Close();
                        }
                    }
                }
            };
        }
    }
}