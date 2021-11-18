using Kuiper.Services;

namespace Kuiper.Systems
{
    public class DestinationsCommand : ShipBaseCommand
    {
        public DestinationsCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService) : base(shipService, eventService, gameTimeService)
        {
        }

        public override string Name => "destinations";

        public override void Execute(string[] args)
        {
            ConsoleWriter.Write("From here you can set a course towards these destinations");
            var destinations = _shipService.GetPossibleDestinations();
            foreach(var destination in destinations)
            {
                ConsoleWriter.Write(destination.Name);
            }
            
        }
    }
} 