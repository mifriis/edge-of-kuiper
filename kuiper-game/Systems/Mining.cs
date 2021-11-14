// using Kuiper.Domain;
// using Kuiper.Services;

// namespace Kuiper.Systems
// {
//     public class Mining
//     {
//         private readonly ICaptainService _service;
//         private Captain _captain;

//         public Mining(ICaptainService service)
//         {
//             _service = service;
//             _captain = _service.GetCaptain();
//         }

//         public string ScanForAsteroidsNearCurrentPlanet()
//         {
//             var ship = _captain.Ship;
//             if(ship.Status == ShipStatus.InOrbit)
//             {
//                 //Add to ship event queue
//                 return $"Scanning initialized. Completion estimated in 4 hours";
//             }
//             return $"{ship.Name} currently under thrust and travelling. It's not possible to scan right now";
//         }

        
//     }
    
// }