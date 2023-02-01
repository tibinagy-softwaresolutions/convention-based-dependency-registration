using Microsoft.Extensions.Caching.Distributed;
using TNArch.DependencyInjection.Convention.Abstractions;

namespace TNArch.DependencyInjection.Convention.Demo.Cache
{
    [ConfigurationDescriptor("CacheConfig")]
    public class DistributedCacheConfig : DistributedCacheEntryOptions
    {
        public TimeSpan ReadTimeout { get; set; }
        public TimeSpan WriteTimeout { get; set; }
    }
}