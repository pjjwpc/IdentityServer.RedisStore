using System;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Host.Cors {
    public static class IIdentityServerBuilderExtensions {
        public static IIdentityServerBuilder AllowCors(this IIdentityServerBuilder builder, Action<CorsOptions> setupAction) {
            var services = builder.Services;
            if (setupAction != null) {
                services.Configure(setupAction);
            }

            services.AddTransient<ICorsPolicyService, CorsPolicyService>();
            //todo
            //var paths = new List<string>(IdentityServerConstants.ProtocolRoutePaths.CorsPaths);

            //paths.AddRange(CorsOptions.RoutePaths);

            //// just for allow more route paths
            //services.AddTransient<ICorsPolicyProvider>(provider => {
            //    return new PolicyProvider(
            //        provider.GetRequiredService<ILogger<PolicyProvider>>(),
            //        paths,
            //        provider.GetRequiredService<ICorsPolicyService>());
            //});
            services.AddCors();

            return builder;
        }
    }
}
