using System;
using System.Linq;
using Kuiper.Systems.CommandInfrastructure;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICommandParser _commandParser;

        public CaptainsConsole(ICommandParser commandParser)
        {

            _commandParser = commandParser;
        }
       
        public void ConsoleMapper(string input)
        {
            _commandParser.ParseAndExecute(input);
        }
    }
}