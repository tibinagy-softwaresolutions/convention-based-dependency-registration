using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TNArch.DependencyInjection.Convention
{
    public static class DependencyInjectionExtensions
    {
        public static DependencyDescriptorBuilder UseDbContext<TContext>(this DependencyDescriptorBuilder builder, string sqlConnectionConfigKey) where TContext : DbContext
        {
            var retryCount = builder.Configuration.GetValue<int>("TransientFaultHandling:MaxRetryCount");
            var retryDelay = builder.Configuration.GetValue<TimeSpan>("TransientFaultHandling:MaxRetryDelay");

            builder.ServiceCollection.AddDbContext<TContext>((sp, optionBuilder) =>
            {
                var connection = sp.GetRequiredService<IConfiguration>().GetValue<string>(sqlConnectionConfigKey);

                optionBuilder.UseSqlServer(connection, sqlOptions => sqlOptions.EnableRetryOnFailure(retryCount, retryDelay, new int[0]));
            });

            return builder;
        }

        public static DependencyDescriptorBuilder UseCache(this DependencyDescriptorBuilder builder, string redisConfigKey)
        {
            var redisConfigs = builder.Configuration.GetValue<string>(redisConfigKey);

            if (string.IsNullOrEmpty(redisConfigs))
            {
                builder.ServiceCollection.AddDistributedMemoryCache();
            }
            else
            {
                builder.ServiceCollection.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConfigs;
                    options.InstanceName = "AppCache";
                });
            }

            return builder;
        }

        public static DependencyDescriptorBuilder UseApplicationInsights(this DependencyDescriptorBuilder builder, string appInsightsConfigKey)
        {
            var appInsightConnection = builder.Configuration.GetValue<string>(appInsightsConfigKey);

            if (!string.IsNullOrEmpty(appInsightConnection))
                builder.ServiceCollection.AddApplicationInsightsTelemetryWorkerService(o => o.ConnectionString = appInsightConnection);

            return builder;
        }

        public static async Task<T> RunWithinTimeout<T>(this Task<T> @this, TimeSpan timeout)
        {
            await Task.WhenAny(@this, Task.Delay(timeout)).Unwrap();

            return @this.IsCompleted && @this.Exception == null ? @this.Result : default;
        }

        public static async Task RunWithinTimeout(this Task @this, TimeSpan timeout)
        {
            await Task.WhenAny(@this, Task.Delay(timeout)).Unwrap();
        }
    }
}
