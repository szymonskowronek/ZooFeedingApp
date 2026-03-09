namespace ZooFeedingApp.Services.Interfaces
{
    public interface IDataProvider<T>
    {
        Task<T> GetDataAsync();
    }
}