using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kuiper.Domain.Ship;

namespace Kuiper.Services
{
    public interface ITraderService
    {
        List<Ship> GetTraderShips();

    }
}
