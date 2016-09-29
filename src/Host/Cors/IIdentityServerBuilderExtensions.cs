using System;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Hosting.Cors;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Host.Cors {
    public static class IIdentityServerBuilderExtensions {
        public static IIdentityServerBuilder AllowCors(this IIdentityServerBuilder builder, Action<CorsOptions> setupAction) {
            var services = builder.Services;
            if (setupAction != null) {
                services.Configure(setupAction);
            }

            services.AddTransient<ICorsPolicyService, CorsPolicyService>();

            services.AddCors();

            return builder;
        }
    }
}
