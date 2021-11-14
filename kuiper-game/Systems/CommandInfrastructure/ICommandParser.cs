using System;

namespace Kuiper.Systems.CommandInfrastructure
{
    public interface ICommandParser
    {
        (Action<string[]> command, string[] args) Parse(string input);

        void ParseAndExecute(string input);
    }
} 