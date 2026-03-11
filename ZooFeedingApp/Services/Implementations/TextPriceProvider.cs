using Microsoft.Extensions.Options;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Interfaces;

namespace ZooFeedingApp.Services.Implementations;

public class TextPriceProvider(IOptions<InputDataOptions> options) : IPriceProvider
{
    private readonly string _filePath = options.Value.PricesFilePath;

    public async Task<IDictionary<FoodCategory, decimal>> GetDataAsync()
    {
        var prices = new Dictionary<FoodCategory, decimal>();
        var lines = await File.ReadAllLinesAsync(_filePath);

        foreach (var line in lines)
        {
            var priceParts = line.Split('=', StringSplitOptions.TrimEntries);
            if (priceParts.Length == 2 && Enum.TryParse<FoodCategory>(priceParts[0], false, out var foodCategory))
            {
                prices[foodCategory] = decimal.Parse(priceParts[1]);
            }
        }

        return prices;
    }
}