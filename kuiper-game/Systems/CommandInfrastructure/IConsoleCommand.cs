namespace Kuiper.Systems
{
    public interface IConsoleCommand
    {
        string Group { get; }
        string Name { get;  }
        void Execute(string[] args);
    }
}
