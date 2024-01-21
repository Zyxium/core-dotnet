using Microsoft.Extensions.Configuration;

namespace Core.DotNet.Extensions.Utilities;

public static class ConfigurationExtension
{
    public static IConfigurationBuilder CreateConfigurationBuilder(
        string settingProjectDirectory,
        string settingJsonFile = "appsettings.json")
    {
        return new ConfigurationBuilder().SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", settingProjectDirectory)).AddJsonFile(settingJsonFile, false, true);
    }
}