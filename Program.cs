using System;
using Kuiper.Systems;

namespace edge_of_kuiper
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new CaptainsConsole();
            Console.ReadKey(true);
            Console.WriteLine("Press ESC to stop");
            do 
            {
                while (! Console.KeyAvailable) 
                {
                    console.ConsoleMapper(Console.ReadLine());                          
                }       
            } 
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            Console.ReadLine();
        }
    }
}
