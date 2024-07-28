using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;

namespace CSSharpUtils.Utils;

/// <summary>
/// Provides utility methods for accessing server information.
/// </summary>
public static class ServerUtils
{
    private delegate nint CNetworkSystem_UpdatePublicIp(nint a1);
    private static CNetworkSystem_UpdatePublicIp? _networkSystemUpdatePublicIp;
    
    /// <summary>
    /// Gets the IP address of the server.
    /// </summary>
    /// <returns>The IP address of the server.</returns>
    /// <remarks>Taken from https://github.com/daffyyyy/CS2-SimpleAdmin/blob/main/Helper.cs#L450</remarks>
    public static string GetServerIp()
    {
        var networkSystem = NativeAPI.GetValveInterface(0, "NetworkSystemVersion001");

        unsafe
        {
            if (_networkSystemUpdatePublicIp is null)
            {
                var funcPtr = *(nint*)(*(nint*)(networkSystem) + 256);
                _networkSystemUpdatePublicIp = Marshal.GetDelegateForFunctionPointer<CNetworkSystem_UpdatePublicIp>(funcPtr);
            }
            
            // + 4 to skip type, because the size of uint32_t is 4 bytes
            var ipBytes = (byte*)(_networkSystemUpdatePublicIp(networkSystem) + 4);
            // port is always 0, use the one from convar "hostport"
            return $"{ipBytes[0]}.{ipBytes[1]}.{ipBytes[2]}.{ipBytes[3]}";
        }
    }
}