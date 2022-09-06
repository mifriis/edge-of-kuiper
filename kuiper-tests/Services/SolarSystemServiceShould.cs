using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;
using Kuiper.Domain.Mining;

namespace Kuiper.Tests.Unit.Services
{
    public class SolarSystemServiceShould
    {
        private List<CelestialBody> createTestData() {
            var sun = CelestialBody.Create("Sun", 0,0,0,null,CelestialBodyType.Star);
            var mercury = CelestialBody.Create("Mercury", 0.387, 47.4, 30, sun, CelestialBodyType.Planet);
            var venus = CelestialBody.Create("Venus", 0.723, 35.4, 170, sun, CelestialBodyType.Planet);
            var earth = CelestialBody.Create("Earth", 1, 29.8, 170, sun, CelestialBodyType.Planet);
            var moon = CelestialBody.Create("Luna", 0.00257356604, 1.022, 125, earth, CelestialBodyType.Moon);
            var mars = CelestialBody.Create("Mars", 1.523, 24.1, 95, sun, CelestialBodyType.Planet);
            var jupiter = CelestialBody.Create("Jupiter", 5.205, 13.1, 45, sun, CelestialBodyType.GasGiant);
            var saturn = CelestialBody.Create("Saturn", 9.582, 9.7, 345, sun, CelestialBodyType.Planet);

            var testData = new List<CelestialBody>(){
                sun, mercury, venus, earth, moon, mars, jupiter, saturn
            };

            return testData;
        }

        [Fact]
        public void ReturnBodyBasedOnName()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();

            //Act
            var body = solarSystemService.GetBody("Saturn");

            //Assert
            Assert.Equal(testData.SingleOrDefault(b => b.Name == "Saturn"), body);
        }
        
        [Fact]
        public void ReturnAsteroidBasedOnName()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();

            var asteroid = new Asteroid(AsteroidType.S, AsteroidSize.Tiny, 10,2,2,2,testData.First());
            solarSystemService.AddAsteroid(asteroid);

            //Act
            var body = solarSystemService.GetAsteroid(asteroid.Name);

            //Assert
            Assert.Equal(body.OrbitRadius, asteroid.OrbitRadius);
        }

        [Fact]
        public void ReturnAllBodiesUnderAStar()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();

            //Act
            var bodies = solarSystemService.GetBodies();

            //Assert
            Assert.Equal(bodies.Count(), 6);
            Assert.True(bodies.FirstOrDefault(x => x.CelestialBodyType == CelestialBodyType.Star) == null);
            Assert.True(bodies.FirstOrDefault(x => x.CelestialBodyType == CelestialBodyType.Moon) == null);
        }

        [Fact]
        public void ReturnSattelitesAroundABody()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();

            var earth = testData.FirstOrDefault(x => x.Name == "Earth");

            //Act
            var sats = solarSystemService.GetSatellites(earth);

            //Assert
            Assert.Equal(sats.Count(), 1);
            Assert.Equal(sats.Select(x => x.CelestialBodyType == CelestialBodyType.Star).Count(),1);
        }

        [Fact]
        public void ReturnCorrectAuDistanceBetweenBodies()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();

            //Act
            var origin = solarSystemService.GetBody("Earth");
            var destination = solarSystemService.GetBody("Mars");

            var distance = solarSystemService.GetDistanceInAu(origin, destination);

            //Assert
            Assert.Equal(1.6427351236343384, distance, 6);
        }

        [Fact]
        public void ReturnCorrectKmDistanceBetweenBodies()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            //Act
            var origin = solarSystemService.GetBody("Earth");
            var destination = solarSystemService.GetBody("Mars");

            var distance = solarSystemService.GetDistanceInKm(origin, destination);

            //Assert
            Assert.InRange(distance, 245749658, 245749660);
        }

        [Fact]
        public void ReturnCorrectKmDistanceBetweenAMoonAndADifferentBody()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            //Act
            var origin = solarSystemService.GetBody("Luna");
            var destination = solarSystemService.GetBody("Mars");

            var distance = solarSystemService.GetDistanceInKm(origin, destination);

            //Assert
            Assert.Equal((double)246098375, distance, 0);
        }
        
        [Fact]
        public void AddABodyToTheSolarSystemSuccessfully()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            //Act

            var body = CelestialBody.Create("MyAsteroid", 2, testData.First(), CelestialBodyType.Asteroid);
            var returnBody = solarSystemService.AddCelestialBody(body);

            //Assert
            Assert.Equal(body.Name,returnBody.Name);
            Assert.Equal(body.Velocity,returnBody.Velocity);
        }
        
        [Fact]
        public void RemoveABodyToTheSolarSystemSuccessfully()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            
            //Act
            var body = CelestialBody.Create("MyAsteroid", 2, testData.First(), CelestialBodyType.Asteroid);
            body = solarSystemService.AddCelestialBody(body);
            solarSystemService.RemoveCelestialBody(body);
            var returnBody = solarSystemService.GetBody(body.Name);

            //Assert
            Assert.Null(returnBody);
        }
        
        [Fact]
        public void CanAddABodyWithSatellitesFromTheSolarSystemSuccessfully()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            
            //Act
            var body = CelestialBody.Create("MyAsteroid", 2, testData.First(), CelestialBodyType.Asteroid);
            var sat1 = CelestialBody.Create("MyAsteroid2", 2, body, CelestialBodyType.Asteroid);
            var sat2 = CelestialBody.Create("MyAsteroid3", 2, body, CelestialBodyType.Asteroid);
            body = solarSystemService.AddCelestialBody(body);
            sat1 = solarSystemService.AddCelestialBody(sat1);
            sat2 = solarSystemService.AddCelestialBody(sat2);
            
            var returnBody = solarSystemService.GetBody(body.Name);
            var returnSat2 = solarSystemService.GetBody(sat2.Name);

            //Assert
            Assert.Equal(returnBody.Satellites.Count, 2);
            Assert.Equal(returnBody.Satellites.First().Name, sat1.Name);
            Assert.Equal(returnSat2.Parent, returnBody);
        }
        
        [Fact]
        public void RemoveABodyThatDoesntExistToTheSolarSystemSuccessfully()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            
            //Act
            var body = CelestialBody.Create("MyAsteroid", 2, testData.First(), CelestialBodyType.Asteroid);
            solarSystemService.RemoveCelestialBody(body);
            var returnBody = solarSystemService.GetBody(body.Name);

            //Assert
            Assert.Null(returnBody);
        }
        
        [Fact]
        public void CanRemoveABodyWithSatellitesFromTheSolarSystemSuccessfully()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            
            //Act
            var body = CelestialBody.Create("MyAsteroid", 2, testData.First(), CelestialBodyType.Asteroid);
            var sat1 = CelestialBody.Create("MyAsteroid2", 2, body, CelestialBodyType.Asteroid);
            var sat2 = CelestialBody.Create("MyAsteroid3", 2, body, CelestialBodyType.Asteroid);
            body = solarSystemService.AddCelestialBody(body);
            sat1 = solarSystemService.AddCelestialBody(sat1);
            sat2 = solarSystemService.AddCelestialBody(sat2);
            
            solarSystemService.RemoveCelestialBody(body);
            
            var returnBody = solarSystemService.GetBody(body.Name);
            var returnSat1 = solarSystemService.GetBody(sat1.Name);
            var returnSat2 = solarSystemService.GetBody(sat2.Name);

            //Assert
            Assert.Null(returnBody);
            Assert.Null(returnSat1);
            Assert.Null(returnSat2);
        }
        
        [Fact]
        public void ExceptionThrownIfAttemptToCreateSameNameBody()
        {
            //Arrange
            var gameTimeService = new Mock<IGameTimeService>();
            gameTimeService.Setup(u => u.ElapsedGameTime).Returns(new TimeSpan(7,0,0,0));

            var testData = createTestData();
            var repository = new Mock<ISolarSystemRepository>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var solarSystemService = new SolarSystemService(repository.Object, gameTimeService.Object);
            solarSystemService.LoadFromRepository();
            
            //Act
            var body = CelestialBody.Create("MyAsteroid", 2, testData.First(), CelestialBodyType.Asteroid);
            var body2 = CelestialBody.Create("MyAsteroid", 2, body, CelestialBodyType.Asteroid);
            body = solarSystemService.AddCelestialBody(body);
            
            //Assert
            Assert.Throws<ArgumentException>(() => solarSystemService.AddCelestialBody(body2));
        }
        
            
            
    }
}