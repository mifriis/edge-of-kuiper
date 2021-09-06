using System;

namespace Kuiper.Systems
{
    public static class ConsoleWriter
    {   
        public static void Write(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(input);
            Console.ResetColor();
        }
    }
}