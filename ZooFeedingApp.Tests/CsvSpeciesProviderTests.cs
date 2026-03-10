using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Implementations;

namespace ZooFeedingApp.Tests
{
    public class CsvSpeciesProviderTests : BaseTest
    {
        [Theory]
        [InlineData("Lion;0.10;meat;", "Lion", 0.10, AnimalType.Carnivore, null)]
        [InlineData("Giraffe;0.08;fruit;", "Giraffe", 0.08, AnimalType.Herbivore, null)]
        [InlineData("Wolf;0.07;both;90%", "Wolf", 0.07, AnimalType.Omnivore, 90.0)]
        [InlineData("Piranha;0.5;both;50%", "Piranha", 0.5, AnimalType.Omnivore, 50.0)]
        public async Task GetDataAsync_ShouldParseCsvRowsCorrectly(string csvLine, string expectedName, double expectedRate,
            AnimalType expectedAnimalType, double? expectedMeatPercent)
        {
            // Arrange
            string tempPath = CreateTempPath("csv");
            await File.WriteAllTextAsync(tempPath, csvLine);

            try
            {
                var options = Options.Create(new InputDataOptions { AnimalsFilePath = tempPath });
                var provider = new CsvSpeciesProvider(options);

                // Act
                var result = await provider.GetDataAsync();

                // Assert
                Assert.True(result.ContainsKey(expectedName));
                var species = result[expectedName];
            
                Assert.Equal(expectedRate, species.Rate);
                Assert.Equal(expectedAnimalType, species.AnimalType);
                Assert.Equal(expectedMeatPercent, species.MeatPercentage);
            }
            finally
            {
                DeleteTempFile(tempPath);
            }
        }
    }
}