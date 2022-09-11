using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kuiper.Services;
using Kuiper.Systems.CommandInfrastructure;
using Terminal.Gui;

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
            try
            {
                var captain = Open();
                _captainService.LoadGame(captain);
                shipService.Ship = _captainService.GetCaptain().Ship;
            }
            catch (ArgumentException)
            {
                _captainService.SetupCaptain();
                shipService.Ship = _captainService.GetCaptain().Ship;
            }
            
        }
       
        public void ConsoleMapper(string input)
        {
            _commandParser.ParseAndExecute(input);
        }
        
        public string Open ()
        {
            Application.Init ();
            var saves = _captainService.FindSaves();
            var list = new ComboBox () { Width = Dim.Fill (), Height = Dim.Fill () };
            list.SetSource (saves.ToList());
            list.OpenSelectedItem += (ListViewItemEventArgs text) => { Application.RequestStop (); };

            var d = new Dialog () { Title = "Select captain", Width = Dim.Percent (50), Height = Dim.Percent (50) };
            d.Add (list);
            Application.Top.Add(d);
            Application.Run ();
            Application.Shutdown ();
            
            var selectedCaptain = list.Text.ToString();
            if (!String.IsNullOrEmpty(selectedCaptain))
            {
                return selectedCaptain;
            }

            throw new ArgumentException("No captain selected");
        }
    }
}