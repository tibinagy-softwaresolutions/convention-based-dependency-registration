using TNArch.DependencyInjection.Convention.Abstractions;

namespace TNArch.DependencyInjection.Convention.Tests
{
    public interface ITestService
    {
    }

    [Dependency(typeof(ITestService))]
    public class TestService : ITestService
    {
        public TestService(ITestDependency dependency)
        {
            Dependency = dependency;
        }

        public ITestDependency Dependency { get; }
    }

    [DecorateDependency(typeof(ITestService), typeof(TestService))]
    public class TestServiceDecorator : ITestService
    {

        public TestServiceDecorator(ITestService decoratedService, ITestDependency dependency)
        {
            DecoratedService = decoratedService;
            Dependency = dependency;
        }

        public ITestService DecoratedService { get; }
        public ITestDependency Dependency { get; }
    }

    [DecorateDependency(typeof(ITestService), typeof(TestService), scope: "Scope1")]
    [DecorateDependency(typeof(ITestService), typeof(TestServiceDecorator), scope: "Scope2")]
    public class TestServiceDecoratorScoped : ITestService
    {
        public TestServiceDecoratorScoped(ITestService decoratedService)
        {
            DecoratedService = decoratedService;
        }

        public ITestService DecoratedService { get; }
    }
}
