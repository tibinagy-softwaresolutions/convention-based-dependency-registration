namespace TNArch.DependencyInjection.Convention.Demo.Repository
{
    public interface ITenantRepository
    {
        Task<Tenant[]> GetTenants();

        Task<Tenant> GetTenant(string tenantName);
    }
}