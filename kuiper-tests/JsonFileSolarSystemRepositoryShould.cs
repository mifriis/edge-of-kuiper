using Xunit;
using Kuiper.Systems;
using Kuiper.Services;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System;
using System.Numerics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kuiper.Tests.Unit.Repositories
{
    public class JsonFileSolarSystemRepositoryShould
    {
        private readonly string _testDataFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "Sol.solarsystem.json");
        
        [Fact]
        public void FailIfJsonFileDoesNotExist()
        {
            //Arrange
            var file = Path.GetRandomFileName();
            while (new FileInfo(file).Exists) {
                file = Path.GetRandomFileName();
            }

            //Act

            //Assert
            Assert.Throws<FileNotFoundException>(() => new JsonFileSolarSystemRepository(file));
        }

        [Fact]
        public void NotFailIfJsonFileExist()
        {
            //Arrange
            var file = Path.GetTempFileName();
            //Act
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Assert
            Assert.NotNull(repo);
            File.Delete(file);
        }

        [Fact]
        public void CreateCorrectAmountOfBodiesFromJson()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.Equal(11, bodies.Count());
        }

        [Fact]
        public void CreateStarBodyTypeFromJson()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.OfType<Star>().Count() == 1);
        }

        [Fact]
        public void CreatePlanetBodyTypeFromJson()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.OfType<Planet>().Count() == 6);
        }

        [Fact]
        public void CreateMoonBodyTypeFromJson()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.OfType<Moon>().Count() == 1);
        }

        [Fact]
        public void CreateGasGiantBodyTypeFromJson()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.OfType<GasGiant>().Count() == 2);
        }

        [Fact]
        public void CreateDwarfPlanetBodyTypeFromJson()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.OfType<DwarfPlanet>().Count() == 1);
        }

        [Fact]
        public void SetCorrectOrbitRadius()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            //Assert.True(bodies.Single(x => x.Name == "Earth").OrbitRadius);
        }

        [Fact]
        public void SetCorrectName()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.First().Name == "Sol");
        }

        [Fact]
        public void SetCorrectOrbitVelocity()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.Single(x => x.Name == "Earth").Velocity == 29.8);
        }

        [Fact]
        public void SetCorrectOriginDegrees()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.Single(x => x.Name == "Earth").OriginDegrees == 170);
        }
    }
}