using System.Reflection;
using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CSSharpUtils.Extensions;

/// <summary>
/// Provides extension methods for <see cref="BasePluginConfig"/>.
/// </summary>
public static class ConfigExtensions
{
    // Specifies the options for JSON serialization, including indentation for readability.
    private static readonly JsonSerializerOptions WriteSerializerOptions = new() { WriteIndented = true };
    
    // Specifies the options for JSON deserialization.
    private static readonly JsonSerializerOptions ReadSerializerOptions = new() { ReadCommentHandling = JsonCommentHandling.Skip };

    /// <summary>
    /// Updates the version of the provided configuration object and serializes it back to JSON.
    /// This method ensures that the configuration file reflects the most recent version,
    /// including all properties of the configuration object, even those not initially set.
    /// Also backs up the current configuration file before updating it.
    /// </summary>
    /// <typeparam name="T">The type of the configuration object, must inherit from BasePluginConfig.</typeparam>
    /// <param name="config">The configuration object to update and serialize.</param>
    /// <param name="backup">Should a backup file be created. default: true</param>
    /// <param name="checkVersion">Checks and stops overwriting if configs are the same version. default: true</param>
    /// <returns><c>true</c> if the config is updated; otherwise, <c>false</c>.</returns>
    public static bool Update<T>(this T config, bool backup = true, bool checkVersion = true) where T : BasePluginConfig, new()
    {
        // get config path
        var configPath = GetConfigPath(Assembly.GetCallingAssembly());

        // check if valid config path
        if (configPath == null)
            return false;

        // get newest config version
        var newCfgVersion = new T().Version;

        // loaded config is up-to-date
        if (checkVersion && config.Version == newCfgVersion)
            return false;

        if (backup)
        {
            // get counter of backup file
            var backupCount = GetBackupCount(configPath);

            // create a backup of the current config
            File.Copy($"{configPath}.json", $"{configPath}-{backupCount}.bak", true);
        }

        // update the version
        config.Version = newCfgVersion;

        // serialize the updated config back to json
        var updatedJsonContent = JsonSerializer.Serialize(config, WriteSerializerOptions);
        File.WriteAllText($"{configPath}.json", updatedJsonContent);
        return true;
    }

    /// <summary>
    /// Reloads the configuration from disk, deserializing it back into the configuration object.
    /// </summary>
    /// <typeparam name="T">The type of the configuration object, must inherit from BasePluginConfig.</typeparam>
    /// <returns>The reloaded configuration object.</returns>
    /// <remarks>
    /// You should pass the result of this method to your plugins OnConfigParsed() method.
    /// </remarks>
    public static T Reload<T>(this T config) where T : BasePluginConfig, new()
    {
        // get config path
        var configPath = GetConfigPath(Assembly.GetCallingAssembly());

        // check if valid config path
        if (configPath == null)
            return new();

        // read the configuration file content
        var configContent = File.ReadAllText($"{configPath}.json");

        // deserialize the configuration content back to the object
        return JsonSerializer.Deserialize<T>(configContent, ReadSerializerOptions)!;
    }

    /// <summary>
    /// Gets the full path to the configuration file for the specified plugin.
    /// </summary>
    /// <typeparam name="T">The type of the configuration object, must inherit from BasePluginConfig.</typeparam>
    /// <returns>A string for the full path or <c>null</c></returns>
    public static string? ConfigPath<T>(this T config) where T : BasePluginConfig, new()
    {
        var configPath = GetConfigPath(Assembly.GetCallingAssembly());

        if (configPath != null)
            return $"{configPath}.json";

        return null;
    }

    private static string? GetConfigPath(Assembly callingAssembly)
    {
        var assemblyName = callingAssembly.GetName().Name ?? null;
        return assemblyName == null ? null : $"{Server.GameDirectory}/csgo/addons/counterstrikesharp/configs/plugins/{assemblyName}/{assemblyName}";
    }

    private static int GetBackupCount(string configPath)
    {
        var counter = 0;

        while (File.Exists($"{configPath}-{counter}.bak"))
            counter++;

        return counter;
    }
}