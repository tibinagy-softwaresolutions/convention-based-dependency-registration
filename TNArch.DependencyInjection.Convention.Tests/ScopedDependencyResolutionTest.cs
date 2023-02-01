using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TNArch.DependencyInjection.Convention.Abstractions;
using FluentAssertions;

namespace TNArch.DependencyInjection.Convention.Tests
{
    public class ScopedDependencyResolutionTest
    {
        [Test]
        public void GetRequiredService_DifferentScopes_ServicesResolved()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddService(typeof(ITestService), typeof(TestService), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestService), typeof(TestServiceDecorator), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestService), typeof(TestServiceDecoratorScoped), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependency), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependencyScoped2), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependencyScoped1), DependencyLifeStyle.Scoped);
            serviceCollection.AddScoped<ServiceScope>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<ITestService>();

            service.Should().BeOfType<TestServiceDecorator>();
            (service as TestServiceDecorator).DecoratedService.Should().BeOfType<TestService>();
            (service as TestServiceDecorator).Dependency.Should().BeOfType<TestDependency>();
            ((service as TestServiceDecorator).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependency>();

            var serviceInScope1 = serviceProvider.CreateScope("Scope1").ServiceProvider.GetRequiredService<ITestService>();

            serviceInScope1.Should().BeOfType<TestServiceDecoratorScoped>();
            (serviceInScope1 as TestServiceDecoratorScoped).DecoratedService.Should().BeOfType<TestService>();
            ((serviceInScope1 as TestServiceDecoratorScoped).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependencyScoped1>();

            var serviceInScope2 = serviceProvider.CreateScope("Scope2").ServiceProvider.GetRequiredService<ITestService>();

            serviceInScope2.Should().BeOfType<TestServiceDecoratorScoped>();
            (serviceInScope2 as TestServiceDecoratorScoped).DecoratedService.Should().BeOfType<TestServiceDecorator>();
            ((serviceInScope2 as TestServiceDecoratorScoped).DecoratedService as TestServiceDecorator).Dependency.Should().BeOfType<TestDependencyScoped2>();
            ((serviceInScope2 as TestServiceDecoratorScoped).DecoratedService as TestServiceDecorator).DecoratedService.Should().BeOfType<TestService>();
            (((serviceInScope2 as TestServiceDecoratorScoped).DecoratedService as TestServiceDecorator).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependencyScoped2>();

            var serviceInScope3 = serviceProvider.CreateScope("Scope3").ServiceProvider.GetRequiredService<ITestService>();

            serviceInScope3.Should().BeOfType<TestServiceDecorator>();
            (serviceInScope3 as TestServiceDecorator).DecoratedService.Should().BeOfType<TestService>();
            ((serviceInScope3 as TestServiceDecorator).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependency>();
        }

        [Test]
        public void GetMultipleServices_DifferentScopes_ServiceCollectionsResolved()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependency), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependencyScoped3), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependencyScoped2), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependencyScoped1), DependencyLifeStyle.Scoped);
            serviceCollection.AddService(typeof(ITestDependency), typeof(TestDependency2), DependencyLifeStyle.Scoped);
            serviceCollection.AddScoped<ServiceScope>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var services = serviceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();

            services[0].Should().BeOfType<TestDependency>();
            services[1].Should().BeOfType<TestDependency2>();

            var servicesInScope1 = serviceProvider.CreateScope("Scope1").ServiceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();

            servicesInScope1[0].Should().BeOfType<TestDependency2>();
            servicesInScope1[1].Should().BeOfType<TestDependencyScoped1>();
            servicesInScope1[2].Should().BeOfType<TestDependencyScoped3>();

            var servicesInScope2 = serviceProvider.CreateScope("Scope2").ServiceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();

            servicesInScope2[0].Should().BeOfType<TestDependency2>();
            servicesInScope2[1].Should().BeOfType<TestDependencyScoped2>();

            var servicesInScope3 = serviceProvider.CreateScope("Scope3").ServiceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();

            servicesInScope3[0].Should().BeOfType<TestDependency>();
            servicesInScope3[1].Should().BeOfType<TestDependency2>();
        }
    }

    internal static class DependencyInjectionExtensions
    {
        public static void AddService(this IServiceCollection services, Type serviceType, Type implementationType, DependencyLifeStyle lifeStyle)
        {
            implementationType.GetCustomAttributes(true).OfType<DependencyAttribute>().ForEach(a => services.AddService(serviceType, implementationType, lifeStyle, a));
        }
    }
}