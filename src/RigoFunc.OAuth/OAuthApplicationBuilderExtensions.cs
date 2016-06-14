using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RigoFunc.OAuth;

namespace Microsoft.AspNetCore.Builder {
    /// <summary>
    /// OAuth extensions for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class OAuthApplicationBuilderExtensions {
        /// <summary>
        ///  Enables OAuth for the current application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <param name="url">The URL of the OAuth system.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance this method extends.</returns>
        public static IApplicationBuilder UseOAuth(this IApplicationBuilder app) {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var options = app.ApplicationServices.GetRequiredService<IOptions<OAuthServerOptions>>().Value;
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions {
                Authority = options.OAuthUrl,
                RequireHttpsMetadata = false,

                ScopeName = options.ScopeName,
                ScopeSecret = options.ScopeSecret,
                AutomaticAuthenticate = true
            });

            return app;
        }
    }
}
