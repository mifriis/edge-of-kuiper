using System;
using System.Threading;
using System.Threading.Tasks;
using Kuiper.Systems;
using Microsoft.Extensions.Hosting;

namespace Kuiper
{
    public class MainLoopWorker : IHostedService
    {
        private readonly ICaptainsConsole _console;

        public MainLoopWorker(ICaptainsConsole console)
        {
            _console = console;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {            
            do 
            {
                GameTime.RealStartTime = DateTime.Now;
                while (! Console.KeyAvailable) 
                {
                    _console.ConsoleMapper(Console.ReadLine());                          
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