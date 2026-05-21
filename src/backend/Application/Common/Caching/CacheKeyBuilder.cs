using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Application.Common.Caching;

public static class CacheKeyBuilder
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static string Entity(string resource, object id)
        => $"{CacheDefaults.Version}:{Normalize(resource)}:{id}";

    public static string List(string resource)
        => $"{CacheDefaults.Version}:{Normalize(resource)}:list";

    public static string Page(string resource, int page, int size, object? filter = null)
        => $"{CacheDefaults.Version}:{Normalize(resource)}:page:{page}:{size}:{HashFilter(filter)}";

    public static string Search(string resource, object filter)
        => $"{CacheDefaults.Version}:{Normalize(resource)}:search:{HashFilter(filter)}";

    private static string Normalize(string value)
        => value.Trim().ToLowerInvariant();

    private static string HashFilter(object? filter)
    {
        if (filter is null) return "none";

        var json = JsonSerializer.Serialize(filter, SerializerOptions);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes).ToLowerInvariant()[..16];
    }
}
