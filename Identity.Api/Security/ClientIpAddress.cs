using System.Net;

namespace Identity.Api.Security;

public static class ClientIpAddress
{
    public static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (!IPAddress.TryParse(value, out var address)) return value;
        if (address.Equals(IPAddress.IPv6Loopback)) return IPAddress.Loopback.ToString();
        return address.IsIPv4MappedToIPv6
            ? address.MapToIPv4().ToString()
            : address.ToString();
    }
}
