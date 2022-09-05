namespace Kuiper.Domain.Ship
{
    public interface IShipModule
    {
        ModuleSize Size { get; }
        ModuleType Type { get; }
    }
}