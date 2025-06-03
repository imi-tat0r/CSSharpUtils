using CounterStrikeSharp.API.Core;
using System.Runtime.InteropServices;

namespace CSSharpUtils.Utils;

/// <summary>
/// Provides utility methods for accessing server information.
/// </summary>
public static class ServerUtils
{
    private readonly static IntPtr _networkSystem;

    private delegate nint CNetworkSystemUpdatePublicIp(IntPtr networkSystem);
    private static CNetworkSystemUpdatePublicIp? _networkSystemUpdatePublicIp;

    static unsafe ServerUtils()
    {
        _networkSystem = NativeAPI.GetValveInterface(0, "NetworkSystemVersion001");
    }

    /// <summary>
    /// Gets the IP address of the server.
    /// </summary>
    /// <returns>The IP address of the server.</returns>
    /// <remarks>Taken from https://discord.com/channels/1160907911501991946/1233009182857494588</remarks>
    public unsafe static string GetServerIp()
    {
        if (_networkSystemUpdatePublicIp == null)
        {
            var funcPtr = *(nint*)(*(nint*)_networkSystem + 256);
            _networkSystemUpdatePublicIp = Marshal.GetDelegateForFunctionPointer<CNetworkSystemUpdatePublicIp>(funcPtr);
        }

        var ipBytes = (byte*)(_networkSystemUpdatePublicIp(_networkSystem) + 4);
        return $"{ipBytes[0]}.{ipBytes[1]}.{ipBytes[2]}.{ipBytes[3]}";
    }
}