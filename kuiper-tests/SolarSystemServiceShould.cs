using Xunit;
using Kuiper.Systems;
using Kuiper.Services;
using Kuiper.Repositories;
using Kuiper.Domain.CelestialBodies;
using System;
using System.Numerics;
using System.IO;

namespace Kuiper.Tests.Unit.Services
{
    public class SolarSystemServiceShould
    {
        [Fact]
        public void ReturnBodyBasedOnName()
        {
            //Arrange
            var jsonFile = new FileInfo(Path.GetRandomFileName());
            var repo = new JsonFileSolarSystemRepository(jsonFile);
            var service = new SolarSystemService(repo);

            var jsonString = "{'position':1,'name':'Sol','distance':0,'velocity':0,'originDegrees':0,'type':'Star','satellites':[{'position':1,'name':'Mercury','distance':0.387,'velocity':47.4,'originDegrees':30,'type':'Planet','satellites':[]},{'position':3,'name':'Earth','distance':1,'velocity':29.8,'originDegrees':170,'type':'Planet','satellites':[{'position':1,'name':'Luna','distance':0.00257356604,'velocity':1.022,'originDegrees':125,'type':'Moon'}]}]}";
            repo.GetBodiesFromJson(jsonString);

            //Act

            //Assert
            

            File.Delete(jsonFile.FullName);
        }
    }
}