using ZooFeedingApp.Models;

namespace ZooFeedingApp.Services.Interfaces
{
    public interface ISpeciesProvider : IDataProvider<IDictionary<string, AnimalSpecies>>
    {
    }
}