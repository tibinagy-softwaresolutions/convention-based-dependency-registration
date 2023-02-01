using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TNArch.DependencyInjection.Convention;
using TNArch.DependencyInjection.Convention.Abstractions;

Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddConventionalDependencies("TNArch.DependencyInjection",
                                                c => c.WithConfiguration(hostContext.Configuration)
                                                      .DecoratedWith<DependencyAttribute>()
                                                      .DecoratedWith<DecorateDependencyAttribute>()
                                                      .DecoratedWith<ReplaceDependencyAttribute>()
                                                      .UseCache("Redis:ConnectionString")
                                                      .UseApplicationInsights("ApplicationInsights:ConnectionString")
                                                      .DecoratedWithConfigurationDescriptor());

                services.AddHostedService<HostedService>();

            }).Build().Start();
