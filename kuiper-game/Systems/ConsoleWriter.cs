using System;

namespace Kuiper.Systems
{
    public static class ConsoleWriter
    {
        static int origRow;
        static int origCol;
        
        static ConsoleWriter()
        {
            origRow = Console.CursorTop;
            origCol = Console.CursorLeft;
        }
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
        
        public static void WriteAt(string input, int x, int y, string color)
        {
            
            try
            {
                Console.SetCursorPosition(origCol+x, origRow+y);
                ConsoleWriter.Write(input,color);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
    }
}