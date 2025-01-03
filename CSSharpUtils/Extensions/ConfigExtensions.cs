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
    // Holds the name of the executing assembly, used in constructing the configuration file path.
    private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "";

    // Defines the path to the configuration file, based on the game directory and assembly name.
    public static readonly string ConfigPath =
        $"{Server.GameDirectory}/csgo/addons/counterstrikesharp/configs/plugins/{AssemblyName}/{AssemblyName}";

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
    /// <returns><c>true</c> if the config is updated; otherwise, <c>false</c>.</returns>
    public static bool Update<T>(this T config) where T : BasePluginConfig, new()
    {
        // get newest config version
        var newCfgVersion = new T().Version;

        // loaded config is up-to-date
        if (config.Version == newCfgVersion)
            return false;

        // get counter of backup file
        var backupCount = GetBackupCount();

        // create a backup of the current config
        File.Copy($"{ConfigPath}.json", $"{ConfigPath}-{backupCount}.bak", true);

        // update the version
        config.Version = newCfgVersion;

        // serialize the updated config back to json
        var updatedJsonContent = JsonSerializer.Serialize(config, WriteSerializerOptions);
        File.WriteAllText(ConfigPath, updatedJsonContent);
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
        // read the configuration file content
        var configContent = File.ReadAllText($"{ConfigPath}.json");

        // deserialize the configuration content back to the object
        return JsonSerializer.Deserialize<T>(configContent, ReadSerializerOptions)!;
    }

    private static int GetBackupCount()
    {
        var counter = 0;

        while (File.Exists($"{ConfigPath}-{counter}.bak"))
            counter++;

        return counter;
    }
}