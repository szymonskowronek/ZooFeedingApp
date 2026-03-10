using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Extensions;
using ZooFeedingApp.Services.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<InputDataOptions>(
    builder.Configuration.GetSection("ZooData")
);

builder.Services.AddZooServices();

using var host = builder.Build();

await RunZooApp(host.Services);

async Task RunZooApp(IServiceProvider services)
{
    var priceProvider = services.GetRequiredService<IPriceProvider>();
    var prices = await priceProvider.GetDataAsync();
    var zooLoader = services.GetRequiredService<IZooLoader>();
    var animals = await zooLoader.GetDataAsync();
    
    var calculator = services.GetRequiredService<IFeedingCalculator>();
    decimal totalDailyCost = calculator.CalculateTotalDailyCost(animals, prices);

    Console.WriteLine($"Total Daily Zoo Feeding cost: {totalDailyCost:C}");
}