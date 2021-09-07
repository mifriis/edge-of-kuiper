using System;
using System.Threading;
using System.Threading.Tasks;
using Kuiper.Systems;
using Microsoft.Extensions.Hosting;

namespace Kuiper
{
    public class MainLoopWorker : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
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

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}