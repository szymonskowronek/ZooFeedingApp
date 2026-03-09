using ZooFeedingApp.Models;

namespace ZooFeedingApp.Services.Interfaces
{
    public interface IZooLoader : IDataProvider<IEnumerable<ZooAnimal>>
    {
    }
}