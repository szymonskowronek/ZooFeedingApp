using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Interfaces;

namespace ZooFeedingApp.Services.Implementations;

public class XmlZooLoader(IOptions<InputDataOptions> options, ISpeciesProvider speciesProvider, ILogger<XmlZooLoader> logger) : IZooLoader
{
    private readonly string _filePath = options.Value.ZooFilePath;
    private readonly ISpeciesProvider _speciesProvider = speciesProvider;
    private readonly ILogger<XmlZooLoader> _logger = logger;

    public async Task<IEnumerable<ZooAnimal>> GetDataAsync()
    {
        _logger.LogInformation("Starting to load zoo data from {Path}", _filePath);
        var speciesMap = await _speciesProvider.GetDataAsync();

        var animals = new List<ZooAnimal>();

        var document = await Task.Run(() => XDocument.Load(_filePath));

        var animalElements = GetAnimalElements(document);

        animals = ParseAnimals(animalElements, speciesMap);
        
        _logger.LogInformation("Successfully loaded {Count} animals from XML.", animals.Count);
        return animals;
    }

    private IEnumerable<XElement> GetAnimalElements(XDocument document)
    {
        return document.Descendants()
            .Where(e => e.Attribute("name") != null && e.Attribute("kg") != null);
    }

    private List<ZooAnimal> ParseAnimals(IEnumerable<XElement> elements, IDictionary<string, AnimalSpecies> speciesMap)
    {
        var animals = new List<ZooAnimal>();

        foreach (var element in elements)
        {
            string speciesName = element.Name.LocalName;
            string name = element.Attribute("name")?.Value ?? "Unknown";

            if (double.TryParse(element.Attribute("kg")?.Value, out double weight))
            {
                if (speciesMap.TryGetValue(speciesName, out var species))
                {
                    animals.Add(new ZooAnimal(name, weight, species));
                    _logger.LogDebug("Animal {Name} ({Species}) successfully added", name, speciesName);
                }
                else
                {
                    _logger.LogWarning("Species {Species} not found in species map. Skipping {Name}.", speciesName, name);
                }
            }
        }

        return animals;
    }
}