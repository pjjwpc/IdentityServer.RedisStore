using System;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RigoFunc.IdentityServer.Services.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace RigoFunc.IdentityServer {
    public static class IdentityServerServiceCollectionExtensions {
        [Obsolete("Obsoleted in 1.0.2,use services.AddIdentityServer().AddEntityFrameworkCoreServices", true)]
        public static void AddIdentityServerEntityFrameworkCoreServices(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null) {
            services.AddDbContext<IdentityServerDbContext>(optionsAction);
            services.AddTransient<IScopeStore, EntityFrameworkCoreScopeStore>();
            services.AddTransient<IClientStore, EntityFrameworkCoreClientStore>();
        }

    }
}
