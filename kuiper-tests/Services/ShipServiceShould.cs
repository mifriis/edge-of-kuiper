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
using Kuiper.Systems.Events;
using LamarCodeGeneration.Util;

namespace Kuiper.Tests.Unit.Services
{
    public class ShipServiceShould
    {
        [Fact]
        public void ReturnMergedListOfPlanetMoonsAndAsteroid()
        {
            //Arrange
            var bodies = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Mars" },
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var asteroids = new List<Asteroid>() { 
                new Asteroid(AsteroidType.C,AsteroidSize.Gigantic,2,2,2,2,bodies.First())
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.Asteroids).Returns(asteroids);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = new Ship("The Wichman", new ShipEngine(10000,3,1000000,1100000), 250) { CurrentLocation = currentLocation};

            //Act
            var destinations = shipService.GetPossibleDestinations();

            //Assert
            Assert.Equal(destinations.Count(), 4);
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
            var asteroids = new List<Asteroid>()
            {

            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            eventService.Setup(x => x.GameEvents).Returns(new List<IEvent>());
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.Asteroids).Returns(asteroids);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            solarSystemService.Setup(x => x.GetBody("Mars")).Returns(destination);
            gameTimeService.Setup(u => u.Now()).Returns(now);
            solarSystemService.Setup(u => u.GetDistanceInKm(currentLocation, destination)).Returns(distance);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", engine, 250) { CurrentLocation = currentLocation };
            ship.Modules = new List<IShipModule> {new FuelTank(ModuleSize.Medium)};
            ship.Refuel(50);
            shipService.Ship = ship;
            //Act
            var gameEvent = shipService.SetCourse("Mars");
            //Assert
            Assert.Equal(shipService.Ship.TargetLocation, destination);
            Assert.Equal(shipService.Ship.Status, ShipStatus.Enroute);
            Assert.Equal(1789399, gameEvent.DeltaVSpent,0);
            Assert.Equal(now.AddTicks(5493455925225), gameEvent.EventTime);
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
            var asteroid = new List<Asteroid>() {
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            solarSystemService.Setup(x => x.GetBody("Sovereign")).Returns(target);
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.Asteroids).Returns(asteroid);
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
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", engine, 250) { CurrentLocation = currentLocation, TargetLocation = destination };
            ship.Modules = new List<IShipModule> {new FuelTank(ModuleSize.Medium)};
            ship.Refuel(50);
            shipService.Ship = ship;
            
            //Act
            shipService.FinalizeJourney(deltaVToSpend);

            //Assert
            Assert.Equal(shipService.Ship.CurrentLocation, destination);
            Assert.Equal(shipService.Ship.TargetLocation, null);
            Assert.Equal(shipService.Ship.Status, ShipStatus.InOrbit);
            Assert.Equal(24, shipService.Ship.FuelMass, 0);
        }

        [Fact]
        public void CalculatedVToTarget()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var origin = new CelestialBody() { Name = "Earth" };
            var destination = new CelestialBody() { Name = "Mars" };
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", engine, 250) { CurrentLocation = origin };
            ship.Modules = new List<IShipModule> {new FuelTank(ModuleSize.Medium)};
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            var service = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            service.Ship = ship;

            solarSystemService.Setup(u => u.GetDistanceInKm(origin,destination)).Returns(distance);
            ship.Refuel(50);
            //Act
            var dV = service.CalculateDeltaVForJourney(destination);

            //Assert
            Assert.Equal(1789399, dV, 0);
        }

        [Fact]
        public void CalculateTimeToTarget()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var origin = new CelestialBody() { Name = "Earth" };
            var destination = new CelestialBody() { Name = "Mars" };
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", engine, 250) { CurrentLocation = origin };
            ship.Modules = new List<IShipModule> {new FuelTank(ModuleSize.Medium)};
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            var service = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            service.Ship = ship;
            ship.Refuel(50);

            solarSystemService.Setup(u => u.GetDistanceInKm(origin,destination)).Returns(distance);
            
            //Act
            var timespan = service.CalculateTravelTime(destination);

            //Assert
            Assert.Equal(TimeSpan.FromTicks(5493455925225), timespan);
        }
        
        [Fact]
        public void UpdateShipStatusWhenDestinationIsAsteroid()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var now = DateTime.Now;
            var destination = new Asteroid(AsteroidType.M, AsteroidSize.Gigantic, 2, 2, 2, 2, null);
            var bodies = new List<CelestialBody>() 
            { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() 
            { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var asteroids = new List<Asteroid>()
            {
                destination
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", engine, 250) { CurrentLocation = currentLocation };
            ship.Modules = new List<IShipModule> {new FuelTank(ModuleSize.Medium)};
            
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            eventService.Setup(x => x.GameEvents).Returns(new List<IEvent>());
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.Asteroids).Returns(asteroids);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            solarSystemService.Setup(x => x.GetAsteroid(destination.Name)).Returns(destination);
            gameTimeService.Setup(u => u.Now()).Returns(now);
            solarSystemService.Setup(u => u.GetDistanceInKm(currentLocation, destination)).Returns(distance);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = ship;
            ship.Refuel(50);
            
            //Act
            var gameEvent = shipService.SetCourse(destination.Name);

            //Assert
            Assert.Equal(shipService.Ship.TargetLocation, destination);
            Assert.Equal(shipService.Ship.Status, ShipStatus.Enroute);
        }
        
        [Fact]
        public void ShowDestinationsWhenCurrentLocationIsAsteroid()
        {
            //Arrange
            var distance = 245749658; //As calculated by the solarSystemService
            var now = DateTime.Now;
            var destination = new Asteroid(AsteroidType.M, AsteroidSize.Gigantic, 2, 2, 2, 2, null);
            var bodies = new List<CelestialBody>() 
            { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.GasGiant, Name = "Jupiter" }
            };
            var moons = new List<CelestialBody>() 
            { 
                new CelestialBody() { CelestialBodyType = CelestialBodyType.Moon, Name = "Luna" }
            };
            var asteroids = new List<Asteroid>()
            {
                destination
            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", engine, 250) { CurrentLocation = currentLocation };
            ship.Modules = new List<IShipModule> {new FuelTank(ModuleSize.Medium)};
            ship.Refuel(50);
            
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            eventService.Setup(x => x.GameEvents).Returns(new List<IEvent>());
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.Asteroids).Returns(asteroids);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            solarSystemService.Setup(x => x.GetAsteroid(destination.Name)).Returns(destination);
            gameTimeService.Setup(u => u.Now()).Returns(now);
            solarSystemService.Setup(u => u.GetDistanceInKm(currentLocation, destination)).Returns(distance);
            
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            shipService.Ship = ship;
            //Act
            var gameEvent = shipService.SetCourse(destination.Name);

            //Assert
            Assert.Equal(shipService.Ship.TargetLocation, destination);
            Assert.Equal(shipService.Ship.Status, ShipStatus.Enroute);
        }
        
        [Fact]
        public void RemovePreviousDestinationFromGameEventsIfAlreadyEnroute()
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
            var asteroids = new List<Asteroid>()
            {

            };
            var currentLocation = new CelestialBody() { CelestialBodyType = CelestialBodyType.Planet, Name = "Earth" };
            var solarSystemService = new Mock<ISolarSystemService>();
            var gameTimeService = new Mock<IGameTimeService>();
            var eventService = new Mock<IEventService>();
            var gameEvents = new List<IEvent>()
            {
                new SetCourseEvent() { DeltaVSpent = 10,EventName = "Set Course to Venus", EventTime = now.AddDays(14)}
            };
            eventService.Setup(x => x.GameEvents).Returns(gameEvents);
            solarSystemService.Setup(x => x.GetBodies()).Returns(bodies);
            solarSystemService.Setup(x => x.Asteroids).Returns(asteroids);
            solarSystemService.Setup(x => x.GetSatellites(currentLocation)).Returns(moons);
            solarSystemService.Setup(x => x.GetBody("Mars")).Returns(destination);
            gameTimeService.Setup(u => u.Now()).Returns(now);
            solarSystemService.Setup(u => u.GetDistanceInKm(currentLocation, destination)).Returns(distance);
            var shipService = new ShipService(solarSystemService.Object, eventService.Object, gameTimeService.Object);
            var engine = new ShipEngine(10000,3,1000000,1100000);
            var ship = new Ship("The Boolean", engine, 250) { CurrentLocation = currentLocation };
            ship.Modules = new List<IShipModule> {new FuelTank(ModuleSize.Medium)};
            ship.Refuel(50);
            shipService.Ship = ship;
            //Act
            var gameEvent = shipService.SetCourse(destination.Name);
            
            //Assert
            eventService.Verify(x => x.AddEvent(It.IsAny<IEvent>()), Times.Exactly(1));
            eventService.Verify(x => x.RemoveEvent(It.IsAny<IEvent>()), Times.Exactly(1));

        }
    }
}