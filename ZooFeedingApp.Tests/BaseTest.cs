namespace ZooFeedingApp.Tests;

public abstract class BaseTest
{
    protected string CreateTempPath(string extension)
    {
        if (!extension.StartsWith(".")) extension = "." + extension;

        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{extension}");

        return path;
    }

    protected void DeleteTempFile(string tempPath)
    {
        if (File.Exists(tempPath)) File.Delete(tempPath);
    }
}