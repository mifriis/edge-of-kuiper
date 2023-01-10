using System.Collections.Generic;
using Kuiper.Domain.Ship;

namespace Kuiper.Repositories
{
    public interface ITraderShipsRepository
    {
        IEnumerable<Ship> GetTraderShips();
    }
}