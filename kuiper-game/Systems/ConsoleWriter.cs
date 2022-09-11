using System;

namespace Kuiper.Systems
{
    public static class ConsoleWriter
    {   
        public static void Write(string input)
        {
            Write(input, ConsoleColor.Green);
        }

        public static void Write(string input, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(input);
            Console.ResetColor();
        }
        
        public static void Write(string input, string color)
        {
            Enum.TryParse(color, out ConsoleColor enumColor);
            Console.ForegroundColor = enumColor;
            Console.WriteLine(input);
            Console.ResetColor();
        }
    }
}