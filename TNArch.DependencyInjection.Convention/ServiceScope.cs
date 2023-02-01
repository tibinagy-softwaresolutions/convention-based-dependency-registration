using TNArch.DependencyInjection.Convention.Abstractions;

namespace TNArch.DependencyInjection.Convention
{
    [Dependency(typeof(ServiceScope), DependencyLifeStyle.Scoped)]
    public class ServiceScope
    {
        public string Scope { get; set; }
    }
}
