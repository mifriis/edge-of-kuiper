using System.Diagnostics.CodeAnalysis;
using Kuiper.Domain;
using Kuiper.Domain.Ship;
using Kuiper.Services;

namespace Kuiper.Systems
{
    [ExcludeFromCodeCoverage]//Commands are excluded from UnitTests. They should have as little logic as possible, use the services.
    public class LocationCommand : ShipBaseCommand
    {
        public LocationCommand(IShipService shipService, IEventService eventService, IGameTimeService gameTimeService) : base(shipService, eventService, gameTimeService)
        {
        }

        public override string Name => "location";

        public override void Execute(string[] args)
        {
            switch (_shipService.Ship.Status)
            {
                case ShipStatus.Enroute:
                    ConsoleWriter.Write("Your ship is enroute to " + _shipService.Ship.TargetLocation.Name);
                    break;
                case ShipStatus.InOrbit:
                    ConsoleWriter.Write("Your ship is in orbit above " + _shipService.Ship.CurrentLocation.Name);
                    break;
                default:
                    ConsoleWriter.Write("Your ship has been eaten by the Kraken");
                    break;
            }
        }
    }
} 