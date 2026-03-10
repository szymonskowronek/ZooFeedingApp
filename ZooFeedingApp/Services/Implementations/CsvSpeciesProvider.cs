using Microsoft.Extensions.Options;
using ZooFeedingApp.Configuration;
using ZooFeedingApp.Models;
using ZooFeedingApp.Services.Interfaces;

namespace ZooFeedingApp.Services.Implementations;

public class CsvSpeciesProvider(IOptions<InputDataOptions> options) : ISpeciesProvider
{
    private readonly string _filePath = options.Value.AnimalsFilePath;

    public async Task<IDictionary<string, AnimalSpecies>> GetDataAsync()
    {
        var species = new Dictionary<string, AnimalSpecies>();
        var lines = await File.ReadAllLinesAsync(_filePath);

        foreach (var line in lines)
        {
            var speciesParts = line.Split(';', StringSplitOptions.TrimEntries);
            if (speciesParts.Length < 3) continue;

            string name = speciesParts[0];
            double rate = double.Parse(speciesParts[1]);
            string type = speciesParts[2].ToLower();

            var animalType = type switch
            {
                "meat" => AnimalType.Carnivore,
                "fruit" => AnimalType.Herbivore,
                "both" => AnimalType.Omnivore,
                _ => throw new Exception($"Unknown diet: {type}")
            };

            double? meatPercent = null;
            if(animalType == AnimalType.Omnivore && speciesParts.Length >= 4)
            {
                meatPercent = double.Parse(speciesParts[3].Replace("%", ""));
            }

            species[name] = new AnimalSpecies(name, rate, animalType, meatPercent);
        }
        return species;
    }
}