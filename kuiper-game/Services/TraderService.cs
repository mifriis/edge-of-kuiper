using Kuiper.Domain.CelestialBodies;
using Kuiper.Domain.Mining;
using Kuiper.Domain.Ship;
using Kuiper.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuiper.Services
{
    public class TraderService : ITraderService
    {
        public List<Ship> TraderShips { get; set; }

        private ITraderShipsRepository _traderShipsRepository;

        public TraderService(ITraderShipsRepository repository)
        {
            _traderShipsRepository = repository;
        }

        private void LoadFromRepository()
        {
            TraderShips = _traderShipsRepository.GetTraderShips().ToList();
        }

        public List<Ship> GetTraderShips()
        {
            LoadFromRepository();
            return TraderShips;
        }

    }
}
