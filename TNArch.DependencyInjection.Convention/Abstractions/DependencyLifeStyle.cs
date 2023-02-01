namespace TNArch.DependencyInjection.Convention.Abstractions
{
    public enum DependencyLifeStyle : ushort
    {
        Singleton = 0,
        Scoped = 1,
        Transient = 2,
    }
}