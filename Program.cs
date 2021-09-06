using System;
using Kuiper.Systems;

namespace edge_of_kuiper
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new CaptainsConsole();
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
