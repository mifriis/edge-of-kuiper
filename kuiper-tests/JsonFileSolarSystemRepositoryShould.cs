// using Xunit;
// using Kuiper.Systems;
// using Kuiper.Services;
// using Kuiper.Domain.CelestialBodies;
// using Kuiper.Repositories;
// using System;
// using System.Numerics;
// using System.IO;
// using System.Linq;

// namespace Kuiper.Tests.Unit.Repositories
// {
//     public class JsonFileSolarSystemRepositoryShould
//     {
//         [Fact]
//         public void FailIfJsonFileDoesNotExist()
//         {
//             //Arrange
//             var file = Path.GetRandomFileName();
//             while (new FileInfo(file).Exists) {
//                 file = Path.GetRandomFileName();
//             }

//             //Act

//             //Assert
//             Assert.Throws<FileNotFoundException>(() => new JsonFileSolarSystemRepository(new FileInfo(file)));
//         }

//         [Fact]
//         public void NotFailIfJsonFileExist()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));

//             //Assert
//             Assert.NotNull(repo);
//             File.Delete(file);
//         }

//         [Fact]
//         public void CreateCorrectAmountOfBodiesFromJson()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'Sol','distance':0,'velocity':0,'originDegrees':0,'type':'Star','satellites':[{'position':1,'name':'Mercury','distance':0.387,'velocity':47.4,'originDegrees':30,'type':'Planet','satellites':[]},{'position':3,'name':'Earth','distance':1,'velocity':29.8,'originDegrees':170,'type':'Planet','satellites':[{'position':1,'name':'Luna','distance':0.00257356604,'velocity':1.022,'originDegrees':125,'type':'Moon'}]}]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.Equal(4, bodies.Count);

//             File.Delete(file);
//         }

//         [Fact]
//         public void CreateStarBodyTypeFromJson()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'Sol','distance':0,'velocity':0,'originDegrees':0,'type':'Star','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.IsType<Star>(bodies.SingleOrDefault());

//             File.Delete(file);
//         }

//         [Fact]
//         public void CreatePlanetBodyTypeFromJson()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'Sol','distance':0,'velocity':0,'originDegrees':0,'type':'Planet','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.IsType<Planet>(bodies.SingleOrDefault());

//             File.Delete(file);
//         }

//         [Fact]
//         public void CreateMoonBodyTypeFromJson()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'Sol','distance':0,'velocity':0,'originDegrees':0,'type':'Moon','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.IsType<Moon>(bodies.SingleOrDefault());

//             File.Delete(file);
//         }

//         [Fact]
//         public void CreateGasGiantBodyTypeFromJson()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'Sol','distance':0,'velocity':0,'originDegrees':0,'type':'GasGiant','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.IsType<GasGiant>(bodies.SingleOrDefault());

//             File.Delete(file);
//         }

//         [Fact]
//         public void CreateDwarfPlanetBodyTypeFromJson()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'Sol','distance':0,'velocity':0,'originDegrees':0,'type':'DwarfPlanet','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.IsType<DwarfPlanet>(bodies.SingleOrDefault());

//             File.Delete(file);
//         }

//         [Fact]
//         public void SetCorrectOrbitRadius()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'Sol','distance':42,'velocity':0,'originDegrees':0,'type':'DwarfPlanet','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.Equal(42, bodies.SingleOrDefault().OrbitRadius);

//             File.Delete(file);
//         }

//         [Fact]
//         public void SetCorrectName()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'TestNamePleaseIgnore','distance':42,'velocity':0,'originDegrees':0,'type':'DwarfPlanet','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.Equal("TestNamePleaseIgnore", bodies.SingleOrDefault().Name);

//             File.Delete(file);
//         }

//         [Fact]
//         public void SetCorrectOrbitVelocity()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'TestNamePleaseIgnore','distance':42,'velocity':67,'originDegrees':0,'type':'DwarfPlanet','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.Equal(67, bodies.SingleOrDefault().Velocity);

//             File.Delete(file);
//         }

//         [Fact]
//         public void SetCorrectOriginDegrees()
//         {
//             //Arrange
//             var file = Path.GetTempFileName();
//             var jsonString = "{'position':1,'name':'TestNamePleaseIgnore','distance':42,'velocity':67,'originDegrees':69,'type':'DwarfPlanet','satellites':[]}";

//             //Act
//             var repo = new JsonFileSolarSystemRepository(new FileInfo(file));
//             var bodies = repo.GetBodiesFromJson(jsonString);

//             //Assert
//             Assert.Equal(69, bodies.SingleOrDefault().OriginDegrees);

//             File.Delete(file);
//         }
//     }
// }