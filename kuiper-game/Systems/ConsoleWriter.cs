using System;
using System.Numerics;

namespace Kuiper.Systems
{
    public static class ConsoleWriter
    {
        static int origRow;
        static int origCol;
        static int InfoBoxMaxWidth = 30;
        static int InfoBoxMaxHeight = Console.WindowHeight;
        static int InfoBoxActualWidth = Console.WindowWidth - InfoBoxMaxWidth;
        
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
            var split = input.Split(System.Environment.NewLine);
            WriteAt(BuildBoxBorder(), InfoBoxActualWidth, 0, "Green");
            WriteAt(BuildBoxBorder(), InfoBoxActualWidth, split.Length+1, "Green");

            var lineCount = 1;
            foreach (var inputLine in split)
            {
                WriteAt("|", InfoBoxActualWidth, lineCount, "Green");
                WriteAt(inputLine + BuildLinePadding(inputLine), InfoBoxActualWidth+1, lineCount, "Green");
                lineCount++;
            }

            CleanInfoBox(split.Length);
        }

        public static void WriteInfoBox(string input, Vector2 cursorPos)
        {
            WriteInfoBox(input);
            Console.SetCursorPosition((int)cursorPos.X, (int)cursorPos.Y);
            Console.CursorVisible = true;
        }

        private static string BuildLinePadding(string input)
        {
            return BuildLinePadding(input.Length);
        }
        
        private static string BuildLinePadding(int input)
        {
            var linePadding = "";
            
            for (int i = 0; i <= InfoBoxMaxWidth - input - 1; i++)
            {
                linePadding += " ";
            }

            return linePadding;
        }
        
        private static string BuildBoxBorder()
        {
            var linePadding = "+";
            
            for (int i = 0; i <= InfoBoxMaxWidth - 2; i++)
            {
                linePadding += "-";
            }

            return linePadding;
        }

        private static void CleanInfoBox(int skipLines)
        {
            var padding = BuildLinePadding(0);
            for (int i = skipLines+2; i < InfoBoxMaxHeight-5; i++)
            {
                WriteAt(padding, InfoBoxActualWidth, i, "Green");
            }
            
        }
        
    }
}