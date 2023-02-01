using TNArch.DependencyInjection.Convention.Abstractions;
using TNArch.DependencyInjection.Convention.Demo.Logger;

namespace TNArch.DependencyInjection.Convention.Demo.Repository
{
    [Dependency(typeof(ITenantRepository))]
    public class TenantRepository : ITenantRepository
    {
        private readonly ILogger<ITenantRepository> _logger;

        private Tenant[] tenants = new[] { new Tenant { Name = "T1" }, new Tenant { Name = "T2" } };

        public TenantRepository(ILogger<ITenantRepository> logger)
        {
            _logger = logger;
        }

        public async Task<Tenant> GetTenant(string tenantName)
        {
            _logger.LogInfo("Repository.GetTenant called");
            return tenants.FirstOrDefault(t => t.Name == tenantName);
        }

        public async Task<Tenant[]> GetTenants()
        {
            _logger.LogInfo("Repository.GetTenants called");
            return tenants;
        }
    }
}