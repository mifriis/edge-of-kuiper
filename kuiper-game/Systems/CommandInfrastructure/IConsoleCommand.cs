namespace Kuiper.Systems
{
    public interface IConsoleCommand
    {
        public string Group { get; }
        public string Name { get;  }
        public void Execute(string[] args);
    }
} 