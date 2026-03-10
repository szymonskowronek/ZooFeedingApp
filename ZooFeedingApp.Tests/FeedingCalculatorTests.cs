using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Implementations;

namespace ZooFeedingApp.Tests
{
    public class FeedingCalculatorTests
    {
        private readonly FeedingCalculator _calculator = new();
        
        [Theory]
        [InlineData("Lion", 160, 0.1, AnimalType.Carnivore, null, 160.00)]
        [InlineData("Giraffe", 500, 0.08, AnimalType.Herbivore, null, 200.00)]
        [InlineData("Wolf", 100, 0.07, AnimalType.Omnivore, 90d, 66.50)]
        [InlineData("Piranha", 2, 0.5, AnimalType.Omnivore, 50d, 7.50)]
        public void CalculateTotalDailyCost_AllDiets_ShouldReturnCorrectCost(string speciesName, double weight, double rate, AnimalType animalType, double? meatPercent, decimal expectedCost)
        {
            //Arrange
            var species = new AnimalSpecies(speciesName, rate, animalType, meatPercent);
            var animal = new ZooAnimal("TestAnimal", weight, species);

            var animals = new List<ZooAnimal> { animal };
            var prices = new Dictionary<FoodCategory, decimal>
            {
                { FoodCategory.Meat, 10.00m },
                { FoodCategory.Fruit, 5.00m }
            };

            //Act
            var actualCost = _calculator.CalculateTotalDailyCost(animals, prices);

            //Assert
            Assert.Equal(expectedCost, actualCost);
        }

        [Fact]
        public void Calculate_ShouldThrowException_WhenPriceIsMissing()
        {
            var animal = new ZooAnimal("Simba", 100, new AnimalSpecies("Lion", 0.1, AnimalType.Carnivore, null));
            var emptyPrices = new Dictionary<FoodCategory, decimal>();

            Assert.Throws<KeyNotFoundException>(() => _calculator.CalculateTotalDailyCost([animal], emptyPrices));
        }
    }
}