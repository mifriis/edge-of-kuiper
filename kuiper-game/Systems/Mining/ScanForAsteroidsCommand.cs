using Kuiper.Services;

namespace Kuiper.Systems.Mining;

public class SCanForAsteroidsCommand : MiningBaseCommand
{
    public SCanForAsteroidsCommand(IMiningService mining, IEventService eventService, IGameTimeService gameTimeService) : base(mining, eventService, gameTimeService)
    {
    }

    public override string Name => "scan";

    public override void Execute(string[] args)
    {
        ConsoleWriter.Write("Scan initiated...");
        
        var gameEvent = _miningService.ScanForAsteroids();
        
        ConsoleWriter.Write("Scan will end on " + gameEvent.EventTime);
    }
}