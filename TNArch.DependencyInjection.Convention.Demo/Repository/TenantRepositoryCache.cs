using TNArch.DependencyInjection.Convention.Abstractions;
using TNArch.DependencyInjection.Convention.Demo.Cache;
using TNArch.DependencyInjection.Convention.Demo.Logger;

namespace TNArch.DependencyInjection.Convention.Demo.Repository
{
    [DecorateDependency(typeof(ITenantRepository), typeof(TenantRepository))]
    public class TenantRepositoryCache : ITenantRepository
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<TenantRepositoryCache> _logger;

        public TenantRepositoryCache(ITenantRepository tenantRepository, ICacheService cacheService, ILogger<TenantRepositoryCache> logger)
        {
            _tenantRepository = tenantRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public virtual async Task<Tenant[]> GetTenants()
        {
            _logger.LogInfo("Cache.GetTenants called");
            return await _cacheService.GetOrCreate("Tenants", _tenantRepository.GetTenants);
        }

        public async Task<Tenant> GetTenant(string tenantName)
        {
            _logger.LogInfo("Cache.GetTenant called");
            return (await GetTenants()).Single(t => t.Name == tenantName);
        }
    }

    [ReplaceDependency(typeof(ITenantRepository), typeof(TenantRepository), "V2")]
    public class TenantRepositoryV2 : ITenantRepository
    {
        private readonly ILogger<ITenantRepository> _logger;

        private Tenant[] tenants = new[] { new Tenant { Name = "T1V2" }, new Tenant { Name = "T2V2" } };

        public TenantRepositoryV2(ILogger<ITenantRepository> logger)
        {
            _logger = logger;
        }

        public async Task<Tenant> GetTenant(string tenantName)
        {
            _logger.LogInfo("RepositoryV2.GetTenant called");
            return tenants.FirstOrDefault(t => t.Name == tenantName);
        }

        public async Task<Tenant[]> GetTenants()
        {
            _logger.LogInfo("RepositoryV2.GetTenants called");
            return tenants;
        }
    }
}