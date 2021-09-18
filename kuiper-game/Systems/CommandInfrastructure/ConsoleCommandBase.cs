namespace Kuiper.Systems
{
    public abstract class ConsoleCommandBase : IConsoleCommand
    {
        public virtual string Group { get; }

        public abstract string CommandName { get; }

        public abstract void Execute(string[] args);
    }
}