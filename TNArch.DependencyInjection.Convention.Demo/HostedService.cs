using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TNArch.DependencyInjection.Convention;
using TNArch.DependencyInjection.Convention.Demo.Logger;
using TNArch.DependencyInjection.Convention.Demo.Repository;

public class HostedService : IHostedService
{
    private readonly ILogger<HostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public HostedService(ILogger<HostedService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInfo("Start worker");

        using (var scope = _serviceProvider.CreateScope())
        {
            var tenantRepository = scope.ServiceProvider.GetService<ITenantRepository>();
            var tenants = await tenantRepository.GetTenants();

            var tenants2 = await tenantRepository.GetTenants();
        }

        using (var scope = _serviceProvider.CreateScope("V2"))
        {
            var tenantRepository = scope.ServiceProvider.GetService<ITenantRepository>();
            var tenant = await tenantRepository.GetTenant("T1V2");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInfo("Stop worker");

        return Task.CompletedTask;
    }
}