namespace Kuiper.Systems
{
    public interface IConsoleCommand
    {
        public string Group { get; }
        public string CommandName { get;  }
        public void Execute(string[] args);
    }
}