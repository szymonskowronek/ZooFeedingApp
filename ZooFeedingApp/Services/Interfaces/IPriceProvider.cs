using ZooFeedingApp.Models;

namespace ZooFeedingApp.Services.Interfaces;

public interface IPriceProvider : IDataProvider<IDictionary<FoodCategory, decimal>>
{
}