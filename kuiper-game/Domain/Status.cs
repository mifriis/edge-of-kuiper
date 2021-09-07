namespace Kuiper.Domain
{
    public enum ShipStatus
    {
       Enroute,
       InOrbit  
    }

    public static class ShipStatusExtensions
    {
    public static string ToFriendlyString(this ShipStatus status)
    {
        switch(status)
        {
        case ShipStatus.Enroute:
            return "The ship is enroute to it's destination";
        case ShipStatus.InOrbit:
            return "The ship is in orbit above it's location";
        default:
            return "Get your damn dirty hands off me you FILTHY APE!";
        }
    }
    }
}