using System;
using System.Collections.Generic;
using Kuiper.Domain;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICaptainService _captainService;
        private readonly Captain _currentCaptain;
        private Dictionary<string,Action<string[]>> commands = new Dictionary<string,Action<string[]>>();

        public CaptainsConsole(ICaptainService captainService, IEnumerable<ICustomConsoleCommand> commands)
        {
            _captainService = captainService;

            _currentCaptain = _captainService.SetupCaptain();

            foreach(var command in commands)
            {
                var commandName = command.Group == null ? $"{command.CommandName}" : $"{command.Group} {command.CommandName}";
                this.commands[commandName.ToLower()] = (args) => command.Execute(args);
            }
        }

        public void ConsoleMapper(string input)
        {
            switch (input)
            {
                case "help":
                    Help();
                    break;
                default:
                    RunCommand(input);
                    break;
            }
        }

        public void Help()
        {
            ConsoleWriter.Write("Available Commands:");
            foreach (var command in commands)
            {
                ConsoleWriter.Write(command.Key);
            }
        }

        private void RunCommand(string consoleInput)
        {
            var input = consoleInput.ToLower().Split(" ");
            var rollingSegment = "";
            for (int i = 0; i < input.Length; i++)
            {
                rollingSegment += input[i];
                try
                {
                    var argsIndex = i + 1; 
                    commands[rollingSegment].Invoke(input[argsIndex..]);
                    return;
                }
                catch
                {
                    // do nothing
                }
                rollingSegment += " ";
            }

            ConsoleWriter.Write($"{consoleInput} not recognized. Try 'help' for list of commands");
        }
    }
}