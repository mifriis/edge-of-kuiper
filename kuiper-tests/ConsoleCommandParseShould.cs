using Kuiper.Systems;
using Kuiper.Systems.CommandInfrastructure;
using System.Collections.Generic;
using Xunit;

namespace Kuiper.Tests.Unit.Systems
{
    public class TestConsoleCommand : IConsoleCommand
    {
        public string Group => null;

        public string Name => "name";

        public void Execute(string[] args)
        {
            Args = args;
        }

        public string[] Args { get; private set; }
    }

    public class TestGroupedConsoleCommand : IConsoleCommand
    {
        public string Group => "group";

        public string Name => "name";

        public void Execute(string[] args)
        {
            Args = args;
        }

        public string[] Args { get; private set; }
    }

    public class ConsoleCommandParseShould
    {
        [Fact]
        public void Parse_WithNullGroupedCommand_ParsesWithNameAsGroup()
        {
            // Arrange
            var command = new TestConsoleCommand();
            var args = "args";
            var commandName = $"{command.Name} {args}";
            var commands = new List<IConsoleCommand> { command };
            var parser = new ConsoleCommandParser(commands);

            // Act
            var (foundCommand, foundArgs) = parser.Parse(commandName);
            foundCommand.Invoke(foundArgs);

            // Assert
            Assert.Equal(args, foundArgs[0]);
            Assert.Equal(args, command.Args[0]);
        }

        [Fact]
        public void ParseAndExecute_WithNullGroupedCommand_ParsesWithNameAsGroup()
        {
            // Arrange
            var command = new TestConsoleCommand();
            var args = "args";
            var commandName = $"{command.Name} {args}";
            var commands = new List<IConsoleCommand> { command };
            var parser = new ConsoleCommandParser(commands);

            // Act
            parser.ParseAndExecute(commandName);

            // Assert
            Assert.Equal(args, command.Args[0]);
        }

        [Fact]
        public void Parse_WithArgs_ParsesWithArgs()
        {
            // Arrange
            var command = new TestGroupedConsoleCommand();
            var args = "args";
            var commandName = $"{command.Group} --{command.Name} {args}";
            var commands = new List<IConsoleCommand> { command };
            var parser = new ConsoleCommandParser(commands);

            // Act
            var (foundCommand, foundArgs) = parser.Parse(commandName);
            foundCommand.Invoke(foundArgs);

            // Assert
            Assert.Equal(args, foundArgs[0]);
            Assert.Equal(args, command.Args[0]);
        }

        [Fact]
        public void Parse_WithoutArgs_ParsesWithNullArgs()
        {
            // Arrange
            var command = new TestGroupedConsoleCommand();
            var commandName = $"{command.Group} --{command.Name}";
            var commands = new List<IConsoleCommand> { command };
            var parser = new ConsoleCommandParser(commands);

            // Act
            var (foundCommand, foundArgs) = parser.Parse(commandName);
            foundCommand.Invoke(foundArgs);

            // Assert
            Assert.Null(foundArgs);
            Assert.Null(command.Args);
        }

        [Fact]
        public void ParseAndExecute_WithArgs_ExecutesWithArgs()
        {
            // Arrange
            var command = new TestGroupedConsoleCommand();
            var args = "args";
            var commandName = $"{command.Group} --{command.Name} {args}";
            var commands = new List<IConsoleCommand> { command };
            var parser = new ConsoleCommandParser(commands);

            // Act
            parser.ParseAndExecute(commandName);

            // Assert
            Assert.Equal(args, command.Args[0]);
        }

        [Fact]
        public void ParseAndExecute_WithoutArgs_ExecutesWithNullArgs()
        {
            // Arrange
            var command = new TestGroupedConsoleCommand();
            var commandName = $"{command.Group} --{command.Name}";
            var commands = new List<IConsoleCommand> { command };
            var parser = new ConsoleCommandParser(commands);

            // Act
            parser.ParseAndExecute(commandName);

            // Assert
            Assert.Null(command.Args);
        }
    }
}
