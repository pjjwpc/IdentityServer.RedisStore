using System;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RigoFunc.IdentityServer.Services.Redis;

namespace RigoFunc.IdentityServer {
    /// <summary>
    /// Contains extension methods to <see cref="IIdentityServerBuilder"/> for configuring identity server.
    /// </summary>
    public static class IIdentityServerBuilderExtensions {
        public static IServiceCollection AddRedisTransientStores(this IServiceCollection services, Action<RedisStoreOptions> options = null) {
            var redisStoreOptions = new RedisStoreOptions();
            options?.Invoke(redisStoreOptions);

            services.TryAddSingleton(redisStoreOptions);
            services.TryAddSingleton<IAuthorizationCodeStore, RedisAuthorizationCodeStore>();
            services.TryAddSingleton<IRefreshTokenStore, RedisRefreshTokenStore>();
            services.TryAddSingleton<ITokenHandleStore, RedisTokenHandleStore>();
            services.TryAddSingleton<IConsentStore, RedisConsentStore>();
            return services;
        }
    }
}
