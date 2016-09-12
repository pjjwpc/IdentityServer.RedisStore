using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RigoFunc.IdentityServer.DistributedStore;
using RigoFunc.IdentityServer.DistributedStore.Store;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// Contains extension methods to <see cref="IIdentityServerBuilder"/> for configuring identity server.
    /// </summary>
    public static class IIdentityServerBuilderExtensions {
        public static IIdentityServerBuilder AddDistributedStores(this IIdentityServerBuilder builder) {
            var services = builder.Services;

            services.TryAddSingleton<IDataSerializer<PersistedGrant>, PersistedGrantSerializer>();

            services.AddSingleton<IPersistedGrantStore,PersistedGrantStore >();

            return builder;
        }
    }
}
