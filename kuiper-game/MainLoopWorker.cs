using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Kuiper.Systems;
using Microsoft.Extensions.Hosting;

namespace Kuiper
{
    [ExcludeFromCodeCoverage]//Startup code
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