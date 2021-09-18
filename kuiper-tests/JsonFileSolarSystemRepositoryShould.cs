using Xunit;
using Kuiper.Domain.CelestialBodies;
using Kuiper.Repositories;
using System;
using System.IO;
using System.Linq;

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
        public void CreateCorretBodiesFromJson()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.OfType<Star>().Count() == 1, "Incorrect amount of stars.");
            Assert.True(bodies.OfType<Planet>().Count() == 6, "Incorrect amount of planets.");
            Assert.True(bodies.OfType<Moon>().Count() == 1, "Incorrect amount of moons.");
            Assert.True(bodies.OfType<GasGiant>().Count() == 2, "Incorrect amount of GasGiants.");
            Assert.True(bodies.OfType<DwarfPlanet>().Count() == 1, "Incorrect amount of DwarfPlanets.");
        }

        [Fact]
        public void SetCorrectBodyProperties()
        {
            //Arrange
            var repo = new JsonFileSolarSystemRepository(_testDataFilePath);

            //Act
            var bodies = repo.GetSolarSystem();

            //Assert
            Assert.True(bodies.Single(x => x.Name == "Earth").OrbitRadius == 1, "OrbitRadius not correct.");
            Assert.True(bodies.First().Name == "Sol", "Name not correct.");
            Assert.True(bodies.Single(x => x.Name == "Earth").Velocity == 29.8, "Velocity not correct.");
            Assert.True(bodies.Single(x => x.Name == "Earth").OriginDegrees == 170, "OriginDegrees not correct.");
        }
    }
}