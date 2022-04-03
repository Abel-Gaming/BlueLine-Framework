using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using System.Collections.Generic;

namespace BLRP_FRAMEWORK.Menus
{
    public class TicketMenu : BaseScript
    {
        //Selected Items for Ticket Event
        public static string SelectedPlayerID = null;
        private static string SelectedViolation = null;
        private static string SelectedViolationPrice = null;
        private static string SelectedPlayerName = null;

        //Menu Related
        public static List<dynamic> playerList;
        public static List<dynamic> playerNameList;
        public static MenuPool _menuPool;
        public static UIMenu mainMenu;

        public void MainMenuOptions(UIMenu menu)
        {
            playerList = new List<dynamic>();
            playerList.Add("0");
            playerNameList = new List<dynamic>();
            playerNameList.Add("John Doe");

            //Create list items
            var activePlayerNames = new UIMenuListItem("Player", playerNameList, 0);
            var activePlayers = new UIMenuListItem("Player", playerList, 0);

            //Add to menu
            menu.AddItem(activePlayerNames);

            //Do change events
            menu.OnListChange += (sender, item, index) =>
            {
                if (item == activePlayerNames)
                {
                    activePlayers.Index = activePlayerNames.Index; //This sets so the ID will match the selected player
                }
            };

            //Do select events
            menu.OnListSelect += (sender, item, index) =>
            {
                if (item == activePlayerNames)
                {
                    SelectedPlayerName = activePlayerNames.Items[index].ToString();
                    SelectedPlayerID = activePlayers.Items[index].ToString();
                    Screen.ShowNotification($"You have selected ~b~{SelectedPlayerName}~w~ (~y~{SelectedPlayerID}~w~)");
                }
            };
        }

        public void TicketCategories(UIMenu menu)
        {
            var categoriessub = _menuPool.AddSubMenu(menu, "Ticket Categories");
            for (int i = 0; i < 1; i++) ;

            categoriessub.MouseEdgeEnabled = false;
            categoriessub.ControlDisablingEnabled = false;

            // Vehicle Speeding
            var vehiclespeeding = _menuPool.AddSubMenu(categoriessub, "Vehicle Speeding");
            for (int i = 0; i < 1; i++) ;

            vehiclespeeding.MouseEdgeEnabled = false;
            vehiclespeeding.ControlDisablingEnabled = false;

            // Vehicle Operation
            var vehicleoperation = _menuPool.AddSubMenu(categoriessub, "Vehicle Operation");
            for (int i = 0; i < 1; i++) ;

            vehicleoperation.MouseEdgeEnabled = false;
            vehicleoperation.ControlDisablingEnabled = false;

            // Vehicle Equipment
            var vehicleequipment = _menuPool.AddSubMenu(categoriessub, "Vehicle Equipment");
            for (int i = 0; i < 1; i++) ;

            vehicleequipment.MouseEdgeEnabled = false;
            vehicleequipment.ControlDisablingEnabled = false;

            // Documents
            var documents = _menuPool.AddSubMenu(categoriessub, "Documents");
            for (int i = 0; i < 1; i++) ;

            documents.MouseEdgeEnabled = false;
            documents.ControlDisablingEnabled = false;

            // ----------------- Speeding Infractions ---------------- //
            var under15 = new UIMenuItem("Speeding Under 15MPH");
            under15.SetRightLabel("$215");

            var speeding1625 = new UIMenuItem("Speeding Between 16-25 Over");
            speeding1625.SetRightLabel("$360");

            var speeding26 = new UIMenuItem("Speeding Over 26MPH");
            speeding26.SetRightLabel("$480");

            var speeding100 = new UIMenuItem("Speeding Over 100+ MPH");
            speeding100.SetRightLabel("$1,000");

            vehiclespeeding.AddItem(under15);
            vehiclespeeding.AddItem(speeding1625);
            vehiclespeeding.AddItem(speeding26);
            vehiclespeeding.AddItem(speeding100);

            vehiclespeeding.OnItemSelect += (sender, item, index) =>
            {
                SelectedViolation = item.Text;
                SelectedViolationPrice = item.RightLabel;
                Screen.ShowNotification($"You have selected violation ~b~{SelectedViolation}");
            };
            
            // ----------------- Vehicle Operation Infractions ---------------- //
            var atfaultaccident = new UIMenuItem("At Fault in an Accident");
            atfaultaccident.SetRightLabel("$300");

            var carelessdriving = new UIMenuItem("Careless Driving");
            carelessdriving.SetRightLabel("$200");

            var drivingsuspended = new UIMenuItem("Driving While License Suspended");
            drivingsuspended.SetRightLabel("$800");

            var drivingrevoked = new UIMenuItem("Driving While License Revoked");
            drivingrevoked.SetRightLabel("$1,000");

            var dui = new UIMenuItem("Driving Under the Influence");
            dui.SetRightLabel("$1,500");

            var drivingwrongway = new UIMenuItem("Driving Wrong Way");
            drivingwrongway.SetRightLabel("$500");

            var failuretostop = new UIMenuItem("Failure to Stop");
            failuretostop.SetRightLabel("$300");

            var recklessdriving = new UIMenuItem("Reckless Driving");
            recklessdriving.SetRightLabel("$1,000");

            var redlight = new UIMenuItem("Running a Red Light");
            redlight.SetRightLabel("$500");

            var seatbelt = new UIMenuItem("Seat Belt Violation");
            seatbelt.SetRightLabel("$180");

            var violationofrightway = new UIMenuItem("Violation of Right of Way");
            violationofrightway.SetRightLabel("$150");

            vehicleoperation.AddItem(atfaultaccident);
            vehicleoperation.AddItem(carelessdriving);
            vehicleoperation.AddItem(drivingsuspended);
            vehicleoperation.AddItem(dui);
            vehicleoperation.AddItem(drivingwrongway);
            vehicleoperation.AddItem(failuretostop);
            vehicleoperation.AddItem(recklessdriving);
            vehicleoperation.AddItem(redlight);
            vehicleoperation.AddItem(seatbelt);
            vehicleoperation.AddItem(violationofrightway);

            vehicleoperation.OnItemSelect += (sender, item, index) =>
            {
                SelectedViolation = item.Text;
                SelectedViolationPrice = item.RightLabel;
                Screen.ShowNotification($"You have selected violation ~b~{SelectedViolation}");
            };

            // ----------------- Vehicle Equipment Infractions ---------------- //
            var brokenheadlight = new UIMenuItem("Broken Headlight");
            brokenheadlight.SetRightLabel("$250");

            var brokentaillight = new UIMenuItem("Broken Tail Light");
            brokentaillight.SetRightLabel("$250");

            var brokenwindshield = new UIMenuItem("Driving Windshield");
            brokenwindshield.SetRightLabel("$500");

            var noplates = new UIMenuItem("Failure to Display License Plates");
            noplates.SetRightLabel("$300");

            var illegaltint = new UIMenuItem("Illegal Window Tint");
            illegaltint.SetRightLabel("$180");

            var lightingviolation = new UIMenuItem("Lighting Violation");
            lightingviolation.SetRightLabel("$100");

            var turnsignal = new UIMenuItem("Turn Signal Violation");
            turnsignal.SetRightLabel("$100");

            var neonunderglow = new UIMenuItem("Neon / Underglow Lighting");
            neonunderglow.SetRightLabel("$450");

            vehicleequipment.AddItem(brokenheadlight);
            vehicleequipment.AddItem(brokentaillight);
            vehicleequipment.AddItem(brokenwindshield);
            vehicleequipment.AddItem(noplates);
            vehicleequipment.AddItem(illegaltint);
            vehicleequipment.AddItem(lightingviolation);
            vehicleequipment.AddItem(neonunderglow);
            vehicleequipment.AddItem(turnsignal);

            vehicleequipment.OnItemSelect += (sender, item, index) =>
            {
                SelectedViolation = item.Text;
                SelectedViolationPrice = item.RightLabel;
                Screen.ShowNotification($"You have selected violation ~b~{SelectedViolation}");
            };

            // ----------------- Document Infractions ---------------- //
            var drivingwithnolicense = new UIMenuItem("Driving without License");
            drivingwithnolicense.SetRightLabel("$1,000");
            
            var noregistration = new UIMenuItem("Driving without Registration");
            noregistration.SetRightLabel("$500");
            
            var noinsurance = new UIMenuItem("Driving without Insurance");
            noinsurance.SetRightLabel("$1,800");
            
            var expiredinsurance = new UIMenuItem("Expired Vehicle Insurance");
            expiredinsurance.SetRightLabel("$250");
            
            var expiredregistration = new UIMenuItem("Expired Vehicle Registration");
            expiredregistration.SetRightLabel("$250");

            var expiredlicense = new UIMenuItem("Expired Drivers License");
            expiredlicense.SetRightLabel("$200");

            documents.AddItem(drivingwithnolicense);
            documents.AddItem(noregistration);
            documents.AddItem(noinsurance);
            documents.AddItem(expiredinsurance);
            documents.AddItem(expiredregistration);
            documents.AddItem(expiredlicense);

            documents.OnItemSelect += (sender, item, index) =>
            {
                SelectedViolation = item.Text;
                SelectedViolationPrice = item.RightLabel;
                Screen.ShowNotification($"You have selected violation ~b~{SelectedViolation}");
            };
        }

        public void MainMenuOptions2(UIMenu menu)
        {
            var SubmitTicket = new UIMenuItem("~y~Issue Ticket");
            menu.AddItem(SubmitTicket);
            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == SubmitTicket)
                {
                    if (SelectedPlayerID == null)
                    {
                        Screen.ShowNotification("~r~[ERROR]~w~ You must select a player");
                    }
                    else
                    {
                        if (SelectedViolation == null)
                        {
                            Screen.ShowNotification("~r~[ERROR]~w~ You must select a violation");
                        }
                        else
                        {
                            TriggerServerEvent("BLRP_FRAMEWORK:IssueTicket", SelectedPlayerID, SelectedViolationPrice, SelectedViolation);
                            SelectedPlayerID = null;
                            SelectedViolation = null;
                            SelectedViolationPrice = null;
                            mainMenu.Visible = false;
                        }
                    }
                }
            };
        }

        public TicketMenu()
        {
            _menuPool = new MenuPool();
            mainMenu = new UIMenu("Ticket Menu", "BlueLine Framework");
            _menuPool.Add(mainMenu);

            MainMenuOptions(mainMenu);
            TicketCategories(mainMenu);
            MainMenuOptions2(mainMenu);

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
    }
}