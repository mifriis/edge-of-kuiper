using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuiper.Systems.CommandInfrastructure
{
    public class ConsoleCommandParser : ICommandParser
    {
        private Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();

        public ConsoleCommandParser(IEnumerable<IConsoleCommand> commands)
        {

            foreach (var command in commands)
            {
                var commandName = command.Group == null ? $"{command.Name}" : $"{command.Group} --{command.Name}";
                this.commands[commandName.ToLower()] = (args) => command.Execute(args);
            }

            this.commands["help"] = (args) => Help();
        }

        public Action<string[]> this[string commandName]
        {
            get { return commands[commandName]; }
        }

        private void Help()
        {
            ConsoleWriter.Write("Available Commands:");
            foreach (var command in commands)
            {
                ConsoleWriter.Write(command.Key);
            }
        }

        public (Action<string[]> command, string[] args) Parse(string consoleInput)
        {
            var commandNameIndex = consoleInput.IndexOf("--");
            if (commandNameIndex == -1)
            {
                commandNameIndex = 0;
            }

            var commandGroup = consoleInput.Substring(0, commandNameIndex).Trim();
            var commandAndArgs = consoleInput.Substring(commandNameIndex).Split(" ");
            var commandToRun = $"{commandGroup} {commandAndArgs[0]}".Trim();
            var args = commandAndArgs.Length > 1 ? commandAndArgs[1..] : null;

            return (commands[commandToRun], args);
        }

        public void ParseAndExecute(string consoleInput)
        {
            try
            {
                var (command, args) = Parse(consoleInput);
                command.Invoke(args);
            }
            catch
            {
                ConsoleWriter.Write($"{consoleInput} not recognized. Try 'help' for list of commands");
            }
        }
    }
}
