namespace TNArch.DependencyInjection.Convention.Tests
{
    public interface ITestGenericDependency<T>
    {
    }

    public class TestGenericDependency : ITestGenericDependency<int>
    {
        public TestGenericDependency()
        {
        }
    }

    public class TestGenericDependency1 : ITestGenericDependency<bool>
    {
        public TestGenericDependency1()
        {
        }
    }

    public class TestGenericDependency2 : ITestGenericDependency<int>
    {
        public TestGenericDependency2()
        {
        }
    }
}
