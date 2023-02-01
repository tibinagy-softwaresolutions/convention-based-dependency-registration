namespace TNArch.DependencyInjection.Convention.Demo.Cache
{
    public interface ICacheService
    {
        Task<TValue> GetOrCreate<TValue>(string key, Func<Task<TValue>> valueFactory);
        Task RemoveValue(string key);
    }
}