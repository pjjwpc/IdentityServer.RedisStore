using System;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RigoFunc.IdentityServer.Services.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace RigoFunc.IdentityServer {
    public static class IdentityServerBuilderExtension {
        public static IIdentityServerBuilder AddEntityFrameworkCoreServices(this IIdentityServerBuilder builder, Action<DbContextOptionsBuilder> optionsAction = null) {
            var services = builder.Services;
            services.AddDbContext<IdentityServerDbContext>(optionsAction);
            services.AddTransient<IScopeStore, EntityFrameworkCoreScopeStore>();
            services.AddTransient<IClientStore, EntityFrameworkCoreClientStore>();
            return builder;
        }
    }
}
