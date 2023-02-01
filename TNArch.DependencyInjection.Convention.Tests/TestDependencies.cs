using TNArch.DependencyInjection.Convention.Abstractions;

namespace TNArch.DependencyInjection.Convention.Tests
{
    public interface ITestDependency
    {
    }

    [Dependency(typeof(ITestDependency))]
    public class TestDependency : ITestDependency
    {
        public TestDependency()
        {
        }
    }

    [Dependency(typeof(ITestDependency))]
    public class TestDependency2 : ITestDependency
    {
        public TestDependency2()
        {
        }
    }

    [Dependency(typeof(ITestDependency), scope: "Scope1")]
    public class TestDependencyScoped3 : ITestDependency
    {
        public TestDependencyScoped3()
        {
        }
    }

    [ReplaceDependency(typeof(ITestDependency), typeof(TestDependency), scope: "Scope1")]
    public class TestDependencyScoped1 : ITestDependency
    {
        public TestDependencyScoped1()
        {
        }
    }

    [ReplaceDependency(typeof(ITestDependency), typeof(TestDependency), scope: "Scope2")]
    public class TestDependencyScoped2 : ITestDependency
    {
        public TestDependencyScoped2()
        {
        }
    }

    
}
