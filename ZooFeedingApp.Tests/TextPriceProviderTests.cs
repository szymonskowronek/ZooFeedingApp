using Microsoft.Extensions.Options;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Implementations;

namespace ZooFeedingApp.Tests;

public class TextPriceProviderTests : BaseTest
{
    [Theory]
    [InlineData("Meat=12.56\nFruit=5.60", 12.56, 5.60)]
    [InlineData("Meat=10.00\nFruit=2.50", 10.00, 2.50)]
    [InlineData("Fruit=4.25  \n  Meat=15.75", 15.75, 4.25)]
    public async Task GetDataAsync_ShouldParsePricesCorrectly(string fileLines, double expectedMeat, double expectedFruit)
    {
        //Arrange
        string tempPath = CreateTempPath("txt");
        await File.WriteAllTextAsync(tempPath, fileLines);

        try
        {
            var options = Options.Create(new InputDataOptions { PricesFilePath = tempPath });
            var provider = new TextPriceProvider(options);

            // Act
            var prices = await provider.GetDataAsync();

            // Assert
            Assert.Equal((decimal)expectedMeat, prices[FoodCategory.Meat]);
            Assert.Equal((decimal)expectedFruit, prices[FoodCategory.Fruit]);
        }
        finally
        {
            DeleteTempFile(tempPath);
        }
    }

    [Fact]
    public async Task GetDataAsync_ShouldHandleInvalidPriceFormat()
    {
        var tempPath = CreateTempPath("txt"); 
        await File.WriteAllTextAsync(tempPath, "Meat=Twelve");
        var provider = new TextPriceProvider(Options.Create(new InputDataOptions { PricesFilePath = tempPath }));

        await Assert.ThrowsAsync<FormatException>(() => provider.GetDataAsync());
    }
}