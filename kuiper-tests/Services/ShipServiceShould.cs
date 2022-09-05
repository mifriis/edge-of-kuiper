using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;
using Kuiper.Domain;
using Kuiper.Domain.Mining;
using Kuiper.Domain.Ship;

namespace Kuiper.Tests.Unit.Services
{
    public class ShipServiceShould
    {
        [Fact]
        public void ReturnMergedListOfPlanetAndMoons()
        {
            //Arrange
            var bodies = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Mars" },
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = new Ship("The Wichman", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = currentLocation};

            //Act
            var destinations = shipService.GetPossibleDestinations();

            //Assert
            Assert.Equal(destinations.Count(), 3);
        }

        [Fact]
        public void UpdateShipStatusWhenDestinationValid()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var now = DateTime.Now;
            var destination = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Mars" };
            var bodies = new List<CelestialBody>() { 
                destination,
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            solarSystemService.Setup(x => x.GetBody("Mars")).Returns(destination);
            gameTimeService.Setup(u => u.Now()).Returns(now);
            solarSystemService.Setup(u => u.GetDistanceInKm(currentLocation, destination)).Returns(distance);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = new Ship("The Wichman", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = currentLocation};

            //Act
            var gameEvent = shipService.SetCourse("Mars");

            //Assert
            Assert.Equal(shipService.Ship.TargetLocation, destination);
            Assert.Equal(shipService.Ship.Status, ShipStatus.Enroute);
            Assert.Equal(1982926, gameEvent.DeltaVSpent,0);
            Assert.Equal(now.AddTicks(4957314373731), gameEvent.EventTime);
        }

        [Fact]
        public void DoNothingWhenTheDestinationIsFubar()
        {
            //Arrange
            var bodies = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            solarSystemService.Setup(x => x.GetBody("Sovereign")).Returns((CelestialBody)null);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = new Ship("The Wichman", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = currentLocation, Status = ShipStatus.InOrbit};

            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => shipService.SetCourse("Sovereign"));
            Assert.Equal(shipService.Ship.TargetLocation, null);
            Assert.Equal(shipService.Ship.Status, ShipStatus.InOrbit);
        }

        [Fact]
        public void DoNothingWhenTheDestinationIsNotReachable()
        {
            //Arrange
            var target = new CelestialBody() { Name = "Sovereign" };
            var bodies = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            solarSystemService.Setup(x => x.GetBody("Sovereign")).Returns(target);
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = new Ship("The Wichman", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = currentLocation, Status = ShipStatus.InOrbit};

            //Act
            //Assert
            Assert.Throws<ArgumentException>(() => shipService.SetCourse("Sovereign"));
            Assert.Equal(shipService.Ship.TargetLocation, null);
            Assert.Equal(shipService.Ship.Status, ShipStatus.InOrbit);
        }

        [Fact]
        public void UpdateShipStatusWhenDestinationReached()
        {
            //Arrange
            var destination = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Mars" };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            var deltaVToSpend = 1000000;
            shipService.Ship = new Ship("The Wichman", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = currentLocation, TargetLocation = destination, FuelMass = 100};

            //Act
            shipService.FinalizeJourney(deltaVToSpend);

            //Assert
            Assert.Equal(shipService.Ship.CurrentLocation, destination);
            Assert.Equal(shipService.Ship.TargetLocation, null);
            Assert.Equal(shipService.Ship.Status, ShipStatus.InOrbit);
            Assert.Equal(72, shipService.Ship.FuelMass, 0);
        }

        [Fact]
        public void CalculatedVToTarget()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var origin = new CelestialBody() { Name = "Earth" };
            var destination = new CelestialBody() { Name = "Mars" };
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = origin, FuelMass=100 };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            var service = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            service.Ship = ship;

            solarSystemService.Setup(u => u.GetDistanceInKm(origin,destination)).Returns(distance);
            
            //Act
            var dV = service.CalculateDeltaVForJourney(destination);

            //Assert
            Assert.Equal(1675878, dV, 0);
        }

        [Fact]
        public void CalculateTimeToTarget()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var origin = new CelestialBody() { Name = "Earth" };
            var destination = new CelestialBody() { Name = "Mars" };
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = origin, FuelMass=100 };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            var service = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            service.Ship = ship;

            solarSystemService.Setup(u => u.GetDistanceInKm(origin,destination)).Returns(distance);
            
            //Act
            var timespan = service.CalculateTravelTime(destination);

            //Assert
            Assert.Equal(TimeSpan.FromTicks(5865573468979), timespan);
        }
        
        [Fact]
        public void UpdateShipStatusWhenDestinationValidAsteroid()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var now = DateTime.Now;
            var sol = CelestialBody.Create("Sol",0,null,CelestialBodyType.Star);
            var destination = new Asteroid(2, sol, AsteroidType.M, AsteroidSize.Small, 10);
            var bodies = new List<CelestialBody>() {
                sol,
                destination,
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            solarSystemService.Setup(x => x.GetStar()).Returns(sol);
            solarSystemService.Setup(x => x.GetBody(destination.Name)).Returns(destination);
            gameTimeService.Setup(u => u.Now()).Returns(now);
            solarSystemService.Setup(u => u.GetDistanceInKm(currentLocation, destination)).Returns(distance);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = new Ship("The Wichman", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = currentLocation};

            //Act
            var gameEvent = shipService.SetCourse(destination.Name);

            //Assert
            Assert.Equal(shipService.Ship.TargetLocation, destination);
            Assert.Equal(shipService.Ship.Status, ShipStatus.Enroute);
            Assert.Equal(1982926, gameEvent.DeltaVSpent,0);
            Assert.Equal(now.AddTicks(4957314373731), gameEvent.EventTime);
        }
    }
}