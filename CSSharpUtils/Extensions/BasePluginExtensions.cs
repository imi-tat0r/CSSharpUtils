using CounterStrikeSharp.API.Core;

namespace CSSharpUtils.Extensions;

public static class BasePluginExtensions
{
    private static BasePlugin Instance { get; set; } = null!;
    public static BasePlugin PluginInstance => Instance ?? throw new Exception("BasePlugin instance is not initialized!. this.InitializeUtils() must be called in your plugin's Load() function.");
    
    [Obsolete("InitializeUtils is deprecated as it is no longer needed. This method will be removed in a future update.", false)]
    public static void InitializeUtils(this BasePlugin instance)
    {
        Instance = instance;
    }
}