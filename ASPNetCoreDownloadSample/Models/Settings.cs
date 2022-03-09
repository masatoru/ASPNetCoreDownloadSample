using System.IO;
using Microsoft.Extensions.Configuration;

public class Settings
{
    public Settings()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddEnvironmentVariables();
        _instance = builder.Build();
    }

    public static Settings Instance { get; } = new Settings();

    private readonly IConfigurationRoot _instance;

    public string StorageConnectionString => _instance.GetConnectionString(nameof(StorageConnectionString));
}
