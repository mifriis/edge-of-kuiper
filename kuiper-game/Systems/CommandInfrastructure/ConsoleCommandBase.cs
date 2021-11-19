using System.Diagnostics.CodeAnalysis;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Abstract
    public abstract class ConsoleCommandBase : IConsoleCommand
    {
        public virtual string Group { get; }

        public abstract string Name { get; }

        public abstract void Execute(string[] args);
    }
}