using System;
using System.Numerics;

namespace Kuiper.Systems
{
    public static class ConsoleWriter
    {
        static int origRow;
        static int origCol;
        
        static ConsoleWriter()
        {
            Console.CursorVisible = false;
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
        
        public static void WriteInfoBox(string input)
        {
            var maxWidth = 30;
            var boxWidth = Console.WindowWidth - maxWidth;
            var split = input.Split(System.Environment.NewLine);
            
            WriteAt("+", boxWidth, 0, "Green");
            WriteAt("|", boxWidth, 1, "Green");
            WriteAt("|", boxWidth, 2, "Green");
            WriteAt("|", boxWidth, 3, "Green");
            WriteAt("+", boxWidth, 4, "Green");
            WriteAt("-", boxWidth+3, 0, "Green");
            WriteAt("-", boxWidth+2, 0, "Green");
            WriteAt("-", boxWidth+1, 0, "Green");

            var lineCount = 1;
            foreach (var inputLine in split)
            {
                WriteAt(inputLine + BuildLinePadding(inputLine,maxWidth), boxWidth+1, lineCount, "Green");
                lineCount++;
            }
        }

        public static void WriteInfoBox(string input, Vector2 cursorPos)
        {
            WriteInfoBox(input);
            Console.SetCursorPosition((int)cursorPos.X, (int)cursorPos.Y);
            Console.CursorVisible = true;
        }

        private static string BuildLinePadding(string input, int maxWidth)
        {
            var linePadding = "";
            
            for (int i = 0; i <= maxWidth - input.Length - 1; i++)
            {
                linePadding += " ";
            }

            return linePadding;
        }
        
    }
}