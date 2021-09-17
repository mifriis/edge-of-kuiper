using Xunit;
using Kuiper.Repositories;
using System.IO;

namespace Kuiper.Tests.Unit.Repositories
{
    public class JsonFileSolarSystemRepositoryShould
    {
        [Fact]
        public void FailIfJsonFileDoesNotExist()
        {
            //Arrange
            var file = Path.GetRandomFileName();
            while (new FileInfo(file).Exists) 
            {
                file = Path.GetRandomFileName();
            }

            //Act

            //Assert
            Assert.Throws<FileNotFoundException>(() => new JsonFileSolarSystemRepository(new FileInfo(file)));
        }

        [Fact]
        public void NotFailIfJsonFileExist()
        {
            //Arrange
            var file = Path.GetTempFileName();
            //Act
            var repo = new JsonFileSolarSystemRepository(new FileInfo(file));

            //Assert
            Assert.NotNull(repo);
            File.Delete(file);
        }
    }
}