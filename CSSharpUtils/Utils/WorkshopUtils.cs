using CounterStrikeSharp.API.Core;
using System.Runtime.InteropServices;

namespace CSSharpUtils.Utils;

/// <summary>
/// Provides utility methods for accessing workshop-related information.
/// </summary>
public static class WorkshopUtils
{
    private static readonly IntPtr _networkServerService;

    private delegate IntPtr GetGameServerHandle(IntPtr networkServerService);
    private static readonly GetGameServerHandle _getGameServerHandleDelegate;

    private delegate IntPtr GetWorkshopId(IntPtr gameServer);
    private static readonly GetWorkshopId _getWorkshopIdDelegate;

    static unsafe WorkshopUtils()
    {
        _networkServerService = NativeAPI.GetValveInterface(0, "NetworkServerService_001");

        var gameServerOffset = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 24 : 23;
        IntPtr* gameServerHandle = (*(IntPtr**)_networkServerService + gameServerOffset);
        _getGameServerHandleDelegate = Marshal.GetDelegateForFunctionPointer<GetGameServerHandle>(*gameServerHandle);

        var networkGameServer = _getGameServerHandleDelegate(_networkServerService);
        IntPtr* workshopHandle = (*(IntPtr**)networkGameServer + 25);
        _getWorkshopIdDelegate = Marshal.GetDelegateForFunctionPointer<GetWorkshopId>(*workshopHandle);
    }

    /// <summary>
    /// Gets the workshop ID of the current server.
    /// </summary>
    /// <returns>The workshop ID as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the workshop ID cannot be retrieved.</exception>
    public static unsafe string GetID()
    {
        IntPtr networkGameServer = _getGameServerHandleDelegate(_networkServerService);
        IntPtr result = _getWorkshopIdDelegate(networkGameServer);

        var workshopString = Marshal.PtrToStringAnsi(result);
        return workshopString?.Split(',')[0] ?? throw new InvalidOperationException("Failed to retrieve the workshop ID.");
    }
}