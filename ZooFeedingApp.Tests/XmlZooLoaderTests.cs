using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Implementations;
using ZooFeedingApp.Services.Interfaces;

namespace ZooFeedingApp.Tests;

public class XmlZooLoaderTests : BaseTest
{
    [Fact]
    public async Task GetDataAsync_ShouldLinkXmlAnimalsToSpeciesCorrectly()
    {
        // Arrange
        var mockSpeciesRepo = new Mock<ISpeciesProvider>();
        
        var fakeSpecies = new Dictionary<string, AnimalSpecies>
        {
            { "Lion", new AnimalSpecies("Lion", 0.10, AnimalType.Carnivore, null) },
            { "Wolf", new AnimalSpecies("Wolf", 0.07, AnimalType.Omnivore, 90) }
        };
        mockSpeciesRepo.Setup(provider => provider.GetDataAsync()).ReturnsAsync(fakeSpecies);

        string xmlContent = @"
            <Zoo>
                <Lions>
                    <Lion name=""Simba"" kg=""160"" />
                </Lions>
                <Wolves>
                    <Wolf name=""Pin"" kg=""80"" />
                </Wolves>
            </Zoo>";
        
        string tempPath = CreateTempPath("xml");
        await File.WriteAllTextAsync(tempPath, xmlContent);

        try
        {
            var options = Options.Create(new InputDataOptions { ZooFilePath = tempPath });
            var loader = new XmlZooLoader(options, mockSpeciesRepo.Object, NullLogger<XmlZooLoader>.Instance);

            // Act
            var result = (await loader.GetDataAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            
            var simba = result.First(a => a.Name == "Simba");
            Assert.Equal(160, simba.WeightKg);
            Assert.Equal("Lion", simba.AnimalSpecies.Name);
            
            var pin = result.First(a => a.Name == "Pin");
            Assert.Equal(80, pin.WeightKg);
            Assert.Equal(90, pin.AnimalSpecies.MeatPercentage);
        }
        finally
        {
            DeleteTempFile(tempPath);
        }
    }

    [Fact]
    public async Task GetDataAsync_ShouldIgnoreUnknownSpecies()
    {
        //Arrange
        var mockRepo = new Mock<ISpeciesProvider>();
        mockRepo.Setup(r => r.GetDataAsync()).ReturnsAsync(new Dictionary<string, AnimalSpecies>());

        var xmlContent = @"<Zoo><Dragon name='Drogo' kg='1000'/></Zoo>";
        var tempPath = CreateTempPath("xml");
        await File.WriteAllTextAsync(tempPath, xmlContent);

        var options = Options.Create(new InputDataOptions { ZooFilePath = tempPath });
        var loader = new XmlZooLoader(options, mockRepo.Object, NullLogger<XmlZooLoader>.Instance);

        //Act
        var result = await loader.GetDataAsync();

        //Assert
        Assert.Empty(result);

        DeleteTempFile(options.Value.ZooFilePath);
    }
}