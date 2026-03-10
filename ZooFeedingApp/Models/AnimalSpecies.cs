namespace ZooFeedingApp.Models;

public record AnimalSpecies(string Name, double Rate, AnimalType AnimalType, double? MeatPercentage = null);