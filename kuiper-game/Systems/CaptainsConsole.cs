using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Kuiper.Services;
using Kuiper.Systems.CommandInfrastructure;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICommandParser _commandParser;
        private readonly ICaptainService _captainService;
        private readonly IShipService shipService;

        public CaptainsConsole(ICommandParser commandParser, ICaptainService captainService, IShipService shipService)
        {
            Console.OutputEncoding = Encoding.Default;
            _commandParser = commandParser;
            _captainService = captainService;
            this.shipService = shipService;
            WriteMainMenu();
        }
       
        public void ConsoleMapper(string input)
        {
            _commandParser.ParseAndExecute(input);
        }
        
        private void WriteMainMenu()
        {
            var menuWidth = 100;
            var menuMarginTop = 10;
            var menuMarginLeft = ((Console.WindowWidth - menuWidth) / 2) - 1;
            var menuItems = new List<MenuItem>();
            var saves = _captainService.FindSaves();
            
            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, menuMarginTop, "Green"); //10
            ConsoleWriter.WriteAt("|     Edge of Kuiper - Select your Identity or create a new one", menuMarginLeft, menuMarginTop+1, "Green"); //11
            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, menuMarginTop+2, "Green");//12
            var lineCount = 0;
            foreach (var save in saves)
            {
                lineCount++;
                var linePos = lineCount + menuMarginTop + 2; //13+
                ConsoleWriter.WriteAt("|     " + save, menuMarginLeft, linePos, "Green");
                menuItems.Add(new MenuItem(save,new Vector2(menuMarginLeft+1, linePos)));
            }

            if (!saves.Any())
            {
                lineCount++;
                ConsoleWriter.WriteAt("|     No identities found - Please create a new one", menuMarginLeft, lineCount+ menuMarginTop+2, "Green"); //13
            }
            
            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, lineCount+menuMarginTop+3, "Green"); //14
            ConsoleWriter.WriteAt("|     Create new identity", menuMarginLeft, lineCount+menuMarginTop+4, "Green"); //15
            menuItems.Add(new MenuItem("Create new identity",new Vector2(menuMarginLeft+1, lineCount+menuMarginTop+4)));
            ConsoleWriter.WriteAt(ConsoleWriter.BuildBoxLine(menuWidth), menuMarginLeft, lineCount+menuMarginTop+5, "Green"); //16
            ConsoleKeyInfo input;
            
            var currentMenuItemIndex = 0;
            var selectionIndicator = " ->";
            do
            {
                input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.DownArrow)
                {
                    CleanMenuItems(menuItems);
                    if (currentMenuItemIndex < menuItems.Count()-1)
                    {
                        currentMenuItemIndex++;
                    }
                    else
                    {
                        currentMenuItemIndex = 0;
                    }
                    var nav = menuItems[currentMenuItemIndex];
                    ConsoleWriter.WriteAt(selectionIndicator,(int)nav.Position.X,(int)nav.Position.Y,"Green");
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
                        currentMenuItemIndex = menuItems.Count()-1;
                    }
                    var nav = menuItems[currentMenuItemIndex];
                    ConsoleWriter.WriteAt(selectionIndicator,(int)nav.Position.X,(int)nav.Position.Y,"Green");
                }
                if (input.Key == ConsoleKey.Enter)
                {
                    var nav = menuItems[currentMenuItemIndex];
                    if (nav.Title == "Create new identity")
                    {
                        Console.Clear();
                        _captainService.SetupCaptain();
                        shipService.Ship = _captainService.GetCaptain().Ship;
                        
                        break;
                    }
                    Console.Clear();
                    _captainService.LoadGame(nav.Title);
                    break;
                }
            } while (true); //NO ESCAPE FOR YOU!
        }

        private void CleanMenuItems(List<MenuItem> menuItems)
        {
            foreach (var item in menuItems)
            {
                ConsoleWriter.WriteAt("   ",(int)item.Position.X,(int)item.Position.Y,"Green");
            }
        }

        private class MenuItem
        {
            public MenuItem(string title, Vector2 pos)
            {
                Title = title;
                Position = pos;
            }
            
            public string Title { get; set; }
            public Vector2 Position { get; set; }
        }
    }
}