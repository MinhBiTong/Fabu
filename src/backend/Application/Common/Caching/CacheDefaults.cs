namespace Application.Common.Caching;

public static class CacheDefaults
{
    public const string Version = "v1";
    public static readonly TimeSpan QueryTtl = TimeSpan.FromMinutes(2);
}
