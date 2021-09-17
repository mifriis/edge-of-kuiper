using Xunit;
using Moq;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System.Linq;
using System.Collections.Generic;
using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Kuiper.Domain;

namespace Kuiper.Tests.Unit.Services
{
    public class SolarSystemServiceShould
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        
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
            var testData = createTestData();
            var repository = _fixture.Freeze<Mock<ISolarSystemRepository>>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            var sut = _fixture.Create<SolarSystemService>();

            //Act
            var body = sut.GetBody("Saturn");

            //Assert
            Assert.Equal(testData.SingleOrDefault(b => b.Name == "Saturn"), body);
        }

        [Fact]
        public void ReturnCorrectAuDistanceBetweenBodies()
        {
            //Arrange
            var testData = createTestData();
            var repository = _fixture.Freeze<Mock<ISolarSystemRepository>>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);
            
            TimeService.Init(DateTimeOffset.Now - TimeSpan.FromHours(24));

            var sut = _fixture.Create<SolarSystemService>();

            //Act
            var origin = sut.GetBody("Earth");
            var destination = sut.GetBody("Mars");

            var distance = sut.GetDistanceInAu(origin, destination);

            //Assert
            Assert.Equal(1.5909639596939087, distance, 6);
        }

        [Fact]
        public void ReturnCorrectKmDistanceBetweenBodies()
        {
            //Arrange
            var testData = createTestData();
            var repository = _fixture.Freeze<Mock<ISolarSystemRepository>>();
            repository.Setup(x => x.GetSolarSystem()).Returns(testData);

            TimeService.Init(DateTimeOffset.Now - TimeSpan.FromHours(24));

            var sut = _fixture.Create<SolarSystemService>();

            //Act
            var origin = sut.GetBody("Earth");
            var destination = sut.GetBody("Mars");

            var distance = sut.GetDistanceInKm(origin, destination);

            //Assert
            Assert.InRange(distance, 235049650, 2457249685);
        }
    }
}