using System.Xml.Linq;
using Microsoft.Extensions.Options;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Interfaces;

namespace ZooFeedingApp.Services.Implementations;

public class XmlZooLoader(IOptions<InputDataOptions> options, ISpeciesProvider speciesProvider) : IZooLoader
{
    private readonly string _filePath = options.Value.ZooFilePath;
    private readonly ISpeciesProvider _speciesProvider = speciesProvider;

    public async Task<IEnumerable<ZooAnimal>> GetDataAsync()
    {
        var speciesMap = await _speciesProvider.GetDataAsync();

        var animals = new List<ZooAnimal>();

        var document = await Task.Run(() => XDocument.Load(_filePath));

        var animalElements = document.Descendants().Where(element => element.Attribute("name") != null && element.Attribute("kg") != null);

        foreach (var element in animalElements)
        {
            string speciesName = element.Name.LocalName;
            string name = element.Attribute("name")?.Value ?? "Unknown";

            if(double.TryParse(element.Attribute("kg")?.Value, out double weight))
            {
                if(speciesMap.TryGetValue(speciesName, out var species))
                {
                    animals.Add(new ZooAnimal(name, weight, species));
                }
            }
        }
        return animals;
    }
}