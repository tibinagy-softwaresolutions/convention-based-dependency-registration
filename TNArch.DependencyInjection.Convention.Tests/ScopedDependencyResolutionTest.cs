using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TNArch.DependencyInjection.Convention.Abstractions;
using FluentAssertions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace TNArch.DependencyInjection.Convention.Tests
{
    public class ScopedDependencyResolutionTest
    {
        IServiceProvider _serviceProvider;

        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddConventionalDependencies("TNArch.DependencyInjection.Convention",
                                                           c => c.DecoratedWith<DependencyAttribute>()
                                                                 .DecoratedWith<DecorateDependencyAttribute>()
                                                                 .DecoratedWith<ReplaceDependencyAttribute>());

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Test]
        public void GetRequiredService_NoScopeFilter_DecoratorObjectRetuned()
        {

            var service = _serviceProvider.GetRequiredService<ITestService>();

            service.Should().BeOfType<TestServiceDecorator>();
            (service as TestServiceDecorator).DecoratedService.Should().BeOfType<TestService>();
            (service as TestServiceDecorator).Dependency.Should().BeOfType<TestDependency>();
            ((service as TestServiceDecorator).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependency>();
        }

        [Test]
        public void GetRequiredService_Scope1Filter_ScopedDecoratorObjectRetuned()
        {
            var serviceInScope1 = _serviceProvider.CreateScope("Scope1").ServiceProvider.GetRequiredService<ITestService>();

            serviceInScope1.Should().BeOfType<TestServiceDecoratorScoped>();
            (serviceInScope1 as TestServiceDecoratorScoped).DecoratedService.Should().BeOfType<TestService>();
            ((serviceInScope1 as TestServiceDecoratorScoped).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependencyScoped1>();
        }

        [Test]
        public void GetRequiredService_Scope2Filter_NestedDecoratorObjectRetuned()
        {
            var serviceInScope2 = _serviceProvider.CreateScope("Scope2").ServiceProvider.GetRequiredService<ITestService>();

            serviceInScope2.Should().BeOfType<TestServiceDecoratorScoped>();
            (serviceInScope2 as TestServiceDecoratorScoped).DecoratedService.Should().BeOfType<TestServiceDecorator>();
            ((serviceInScope2 as TestServiceDecoratorScoped).DecoratedService as TestServiceDecorator).Dependency.Should().BeOfType<TestDependencyScoped2>();
            ((serviceInScope2 as TestServiceDecoratorScoped).DecoratedService as TestServiceDecorator).DecoratedService.Should().BeOfType<TestService>();
            (((serviceInScope2 as TestServiceDecoratorScoped).DecoratedService as TestServiceDecorator).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependencyScoped2>();
        }

        [Test]
        public void GetRequiredService_Scope3Filter_DefaultDecoratorObjectRetuned()
        {
            var serviceInScope3 = _serviceProvider.CreateScope("Scope3").ServiceProvider.GetRequiredService<ITestService>();

            serviceInScope3.Should().BeOfType<TestServiceDecorator>();
            (serviceInScope3 as TestServiceDecorator).DecoratedService.Should().BeOfType<TestService>();
            ((serviceInScope3 as TestServiceDecorator).DecoratedService as TestService).Dependency.Should().BeOfType<TestDependency>();
        }

        [Test]
        public void GetMultipleServices_NoScopeFilter_DefaultServicesResolved()
        {            
            var services = _serviceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();

            services.Count().Should().Be(2);
            services[0].Should().BeOfType<TestDependency>();
            services[1].Should().BeOfType<TestDependency2>();
        }

        [Test]
        public void GetMultipleServices_Scope1Filter_DefaultAndScope1ServicesResolved()
        {
            var servicesInScope1 = _serviceProvider.CreateScope("Scope1").ServiceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();

            servicesInScope1.Count().Should().Be(3);
            servicesInScope1[0].Should().BeOfType<TestDependency2>();
            servicesInScope1[1].Should().BeOfType<TestDependencyScoped1>();
            servicesInScope1[2].Should().BeOfType<TestDependencyScoped3>();
        }

        [Test]
        public void GetMultipleServices_Scope2Filter_DefaultAndScope2ServicesResolved()
        {
            var servicesInScope2 = _serviceProvider.CreateScope("Scope2").ServiceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();

            servicesInScope2.Count().Should().Be(2);
            servicesInScope2[0].Should().BeOfType<TestDependency2>();
            servicesInScope2[1].Should().BeOfType<TestDependencyScoped2>();
        }

        [Test]
        public void GetMultipleServices_Scope3Filter_DefaultServicesResolved()
        {
            var servicesInScope3 = _serviceProvider.CreateScope("Scope3").ServiceProvider.GetServices<ITestDependency>().Where(s => s != null).OrderBy(s => s.GetType().Name).ToArray();


            servicesInScope3.Count().Should().Be(2);
            servicesInScope3[0].Should().BeOfType<TestDependency>();
            servicesInScope3[1].Should().BeOfType<TestDependency2>();
        }

        [Test]
        public void RegisterAllImplementingInterface_NoScopeFilter_AllServicesResolved()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddConventionalDependencies("TNArch.DependencyInjection.Convention",
                                                           c => c.ImplementsInterface<ITestDependency>());

            var services = serviceCollection.BuildServiceProvider().GetServices<ITestDependency>().OrderBy(s => s.GetType().Name).ToArray();

            services.Count().Should().Be(5);
            services[0].Should().BeOfType<TestDependency>();
            services[1].Should().BeOfType<TestDependency2>();
            services[2].Should().BeOfType<TestDependencyScoped1>();
            services[3].Should().BeOfType<TestDependencyScoped2>();
            services[4].Should().BeOfType<TestDependencyScoped3>();
        }

        [Test]
        public void RegisterAllImplementingGenericInterface_NoScopeFilter_AllServicesResolved()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddConventionalDependencies("TNArch.DependencyInjection.Convention",
                                                           c => c.ImplementsOpenGenericInterface(typeof(ITestGenericDependency<>))
                                                                 .DecoratedWith<DependencyAttribute>()
                                                                 .DecoratedWith<DecorateDependencyAttribute>()
                                                                 .DecoratedWith<ReplaceDependencyAttribute>());

            var intDependencies = serviceCollection.BuildServiceProvider().GetServices<ITestGenericDependency<int>>().OrderBy(s => s.GetType().Name).ToArray();

            intDependencies.Count().Should().Be(2);
            intDependencies[0].Should().BeOfType<TestGenericDependency>();
            intDependencies[1].Should().BeOfType<TestGenericDependency2>();

            var boolDependenciy = serviceCollection.BuildServiceProvider().GetService<ITestGenericDependency<bool>>();

            boolDependenciy.Should().BeOfType<TestGenericDependency1>();
        }

        [Test]
        public void DecoratedWithConfigurationDescriptor_ConfigObjectBound_ConfigObjectResolved()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"Settings:TestConfigs:Value1", "stringValue"},
                {"Settings:TestConfigs:Value2", "0.0:0:25"},
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();


            var serviceCollection = new ServiceCollection();

            serviceCollection.AddConventionalDependencies("TNArch.DependencyInjection.Convention",
                                                           c => c.WithConfiguration(configuration)
                                                                 .DecoratedWithConfigurationDescriptor());

            var testConfiguration = serviceCollection.BuildServiceProvider().GetService<TestConfiguration>();

            testConfiguration.Value1.Should().Be("stringValue");
            testConfiguration.Value2.Should().Be(TimeSpan.FromSeconds(25));
        }
    }
}