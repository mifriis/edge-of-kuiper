using Kuiper.Domain.Ship;
using Kuiper.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kuiper.Systems.Trader
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
    public class DisplayCommand : BaseTraderCommand
    {
        public DisplayCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService, ICaptainService captainService, ITraderService traderService) : base(shipService, eventService, gameTimeService, captainService, traderService)
        {
        }
        public override string Name => "display";

        public override void Execute(string[] args)
        {
            WriteMainMenu();
        }

        private void WriteMainMenu()
        {
            var menuWidth = 100;
            var menuMarginTop = 10;
            var menuMarginLeft = ((Console.WindowWidth - menuWidth) / 2) - 1;
            var menuItems = new List<MenuItem>();
            var ships = _traderService.GetTraderShips();

            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, menuMarginTop, "Green"); //10
            ConsoleWriter.WriteAt("|    Welcome to the ship trader", menuMarginLeft, menuMarginTop + 1, "Green"); //11
            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, menuMarginTop + 2, "Green");//12
            var lineCount = 0;
            var linePos = 0;
            foreach (var ship in ships)
            {
                if(ship.Name == _captainService.GetCaptain().Ship.Name)
                {
                    continue;
                }
                lineCount++;
                linePos = lineCount + menuMarginTop + 2; //13+
                ConsoleWriter.WriteAt("|     " + ship.Name + "     |     Cost: " + ship.Engine.Cost, menuMarginLeft, linePos, "Green");
                menuItems.Add(new MenuItem(ship, new Vector2(menuMarginLeft + 1, linePos)));
            }

            lineCount++;
            linePos = lineCount + menuMarginTop + 2; //13+
            ConsoleWriter.WriteAt("|     " + _captainService.GetCaptain().Ship.Name + "     |     Cost: 0", menuMarginLeft, linePos, "Green");
            menuItems.Add(new MenuItem(_captainService.GetCaptain().Ship, new Vector2(menuMarginLeft + 1, linePos)));

            if (!ships.Any())
            {
                lineCount++;
                ConsoleWriter.WriteAt("|    We appologize but no new ships are in stocks at the moment", menuMarginLeft, lineCount + menuMarginTop + 2, "Green"); //13

            }

            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, lineCount + menuMarginTop + 3, "Green"); //14
            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, lineCount + menuMarginTop + 5, "Green"); //15
            ConsoleKeyInfo input;

            var currentMenuItemIndex = 0;
            var selectionIndicator = " ->";
            do
            {
                input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.DownArrow)
                {
                    CleanMenuItems(menuItems);
                    if (currentMenuItemIndex < menuItems.Count() - 1)
                    {
                        currentMenuItemIndex++;
                    }
                    else
                    {
                        currentMenuItemIndex = 0;
                    }
                    var nav = menuItems[currentMenuItemIndex];
                    ConsoleWriter.WriteAt(selectionIndicator, (int)nav.Position.X, (int)nav.Position.Y, "Green");
                }
                if (input.Key == ConsoleKey.UpArrow)
                {
                    CleanMenuItems(menuItems);
                    if (currentMenuItemIndex > 0)
                    {
                        currentMenuItemIndex--;
                    }
                    else
                    {
                        currentMenuItemIndex = menuItems.Count() - 1;
                    }
                    var nav = menuItems[currentMenuItemIndex];
                    ConsoleWriter.WriteAt(selectionIndicator, (int)nav.Position.X, (int)nav.Position.Y, "Green");
                }
                if (input.Key == ConsoleKey.Enter)
                {
                    var nav = menuItems[currentMenuItemIndex];
                    if (nav.Ship.Name == _captainService.GetCaptain().Ship.Name)
                    {
                        Console.Clear();

                        break;
                    }
                    Console.Clear();
                    _captainService.GetCaptain().Ship.Name = nav.Ship.Name;
                    _captainService.GetCaptain().Ship.Engine = nav.Ship.Engine;
                    _captainService.GetCaptain().Ship.ShipMass = nav.Ship.ShipMass;
                    _captainService.GetCaptain().Account.Withdraw((decimal)nav.Ship.Engine.Cost, _gameTimeService.Now());
                    break;
                }
            } while (true);
        }

        private void CleanMenuItems(List<MenuItem> menuItems)
        {
            foreach (var item in menuItems)
            {
                ConsoleWriter.WriteAt("   ", (int)item.Position.X, (int)item.Position.Y, "Green");
            }
        }

        private class MenuItem
        {
            public MenuItem(Ship ship, Vector2 pos)
            {
                Ship = ship;
                Position = pos;
            }

            public Ship Ship { get; set; }

            public double Cost { get; set; }
            
            public Vector2 Position { get; set; }
        }
    }
}
