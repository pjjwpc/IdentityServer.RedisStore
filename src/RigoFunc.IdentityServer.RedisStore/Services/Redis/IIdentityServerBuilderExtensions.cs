using System;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace RigoFunc.IdentityServer.Services.Redis {
    /// <summary>
    /// Contains extension methods to <see cref="IIdentityServerBuilder"/> for configuring identity server.
    /// </summary>
    public static class IIdentityServerBuilderExtensions {
        public static IIdentityServerBuilder AddRedisTransientStores(this IIdentityServerBuilder builder, Action<RedisStoreOptions> options = null) {
            var redisStoreOptions = new RedisStoreOptions();
            options?.Invoke(redisStoreOptions);

            var services = builder.Services;

            services.AddSingleton(redisStoreOptions);
            services.AddSingleton<IPersistedGrantStore, RedisPersistedGrantStore>();

            return builder;
        }
    }
}
