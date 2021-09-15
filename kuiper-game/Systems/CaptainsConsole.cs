using System;
using System.Collections.Generic;
using System.Reflection;
using Kuiper.Domain;
using Kuiper.Services;

namespace Kuiper.Systems
{
    public class CaptainsConsole : ICaptainsConsole
    {
        private readonly ICaptainService _captainService;
        private readonly Captain _currentCaptain;
        private Dictionary<string, Dictionary<string, Action>> commands = new Dictionary<string, Dictionary<string, Action>>();

        public CaptainsConsole(ICaptainService captainService, IEnumerable<IConsoleCommand> commands)
        {
            _captainService = captainService;

            _currentCaptain = _captainService.SetupCaptain();

            FindCommands(commands);
        }

        public void ConsoleMapper(string input)
        {
            switch (input)
            {
                case "help":
                    Help();
                    break;
                case "save":
                    Save();
                    break;
                case "time":
                    CurrentTime();
                    break;
                default:
                    RunCommand(input);
                    break;
            }
        }

        public void CurrentTime()
        {
            var currentGameTime = GameTime.Now();
            ConsoleWriter.Write($"The time is currently: {currentGameTime}");
        }

        public void Help()
        {
            ConsoleWriter.Write("Available Commands:");
            foreach (var command in commands)
            {
                foreach (var subCommand in command.Value)
                {
                    ConsoleWriter.Write($"{command.Key} {subCommand.Key}");
                }
            }
        }

        public void Save()
        {
            _currentCaptain.LastLoggedIn = GameTime.Now();
            SaveLoad.SaveGame(_currentCaptain);
            ConsoleWriter.Write($"Game saved successfully.", ConsoleColor.Red);
        }

        private void FindCommands(IEnumerable<IConsoleCommand> commands)
        {
            foreach (var command in commands)
            {
                var type = command.GetType();
                var commandName = ((CommandAttribute)type.GetCustomAttribute(typeof(CommandAttribute))).Name;
                foreach (var method in command.GetType().GetMethods())
                {
                    var choiceName = method.Name.ToLower();
                    var attributes = method.GetCustomAttributes(typeof(CommandAttribute), false);
                    foreach (CommandAttribute attribute in attributes)
                    {
                        if (attribute.Name != string.Empty)
                        {
                            choiceName = attribute.Name.ToLower();
                        }
                        if (!this.commands.ContainsKey(commandName))
                        {
                            this.commands[commandName] = new Dictionary<string, Action>();
                        }

                        Action act = () => method.Invoke(command, null);
                        this.commands[commandName][choiceName] = act;
                    }
                }
            }
        }
        private void RunCommand(string consoleInput)
        {
            try
            {
                var input = consoleInput.Split(" ");
                commands[input[0]][input[1]].Invoke();
            }
            catch (Exception ex)
            {
                ConsoleWriter.Write($"{consoleInput} not recognized. Try 'help' for list of commands");
            }
        }
    }
}