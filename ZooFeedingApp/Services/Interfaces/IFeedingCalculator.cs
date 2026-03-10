using ZooFeedingApp.Models;

namespace ZooFeedingApp.Services.Interfaces
{
    public interface IFeedingCalculator
    {
        decimal CalculateTotalDailyCost(IEnumerable<ZooAnimal> animals, IDictionary<FoodCategory, decimal> prices);
    }
}