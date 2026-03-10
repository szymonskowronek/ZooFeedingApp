using Microsoft.Extensions.DependencyInjection;
using ZooFeedingApp.Services.Implementations;
using ZooFeedingApp.Services.Interfaces;

namespace ZooFeedingApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZooServices(this IServiceCollection services)
    {
        services.AddTransient<IPriceProvider, TextPriceProvider>();
        services.AddTransient<ISpeciesProvider, CsvSpeciesProvider>();
        services.AddTransient<IZooLoader, XmlZooLoader>();
        services.AddTransient<IFeedingCalculator, FeedingCalculator>();

        return services;
    }
}