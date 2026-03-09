using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Interfaces;

namespace ZooFeedingApp.Services.Implementations
{
    public class FeedingCalculator : IFeedingCalculator
    {
        public decimal CalculateTotalDailyCost(IEnumerable<ZooAnimal> animals, IDictionary<FoodCategory, decimal> prices)
        {
            decimal totalCost = 0;

            foreach (var animal in animals)
            {
                double totalFoodKg = animal.WeightKg * animal.AnimalSpecies.Rate;

                totalCost += animal.AnimalSpecies.AnimalType switch
                {
                    AnimalType.Carnivore => (decimal)totalFoodKg * prices[FoodCategory.Meat],
                    AnimalType.Herbivore => (decimal)totalFoodKg * prices[FoodCategory.Fruit],
                    AnimalType.Omnivore => CalculateOmnivoreCost(totalFoodKg, animal.AnimalSpecies, prices),
                    _ => throw new ArgumentOutOfRangeException(nameof(animal.AnimalSpecies.AnimalType), "Unknown animal type")
                };
            }

            return totalCost;
        }

        private decimal CalculateOmnivoreCost(double totalFoodKg, AnimalSpecies animalSpecies, IDictionary<FoodCategory, decimal> prices)
        {
            double meatPercent = animalSpecies.MeatPercentage ?? 0;
            double fruitPercent = 100 - meatPercent;

            decimal meatAmount = (decimal)(totalFoodKg * (meatPercent / 100));
            decimal fruitAmount = (decimal)(totalFoodKg * (fruitPercent/ 100));

            return meatAmount * prices[FoodCategory.Meat] + fruitAmount * prices[FoodCategory.Fruit];
        }
    }
}