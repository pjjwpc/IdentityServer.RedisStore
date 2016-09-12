using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace RigoFunc.IdentityServer {
    /// <summary>
    /// Contains extension methods to <see cref="IIdentityServerBuilder"/> for configuring identity server.
    /// </summary>
    public static class IIdentityServerBuilderExtensions {
        /// <summary>
        /// Configures the Asp.Net Core Identity.
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>IIdentityServerBuilder.</returns>
        public static IIdentityServerBuilder ConfigureAspNetCoreIdentity<TUser>(this IIdentityServerBuilder builder)
            where TUser : class {
            var services = builder.Services;

            services.AddScoped<SignInManager<TUser>, IdentityServerSignInManager<TUser>>();
            services.AddTransient<IProfileService, IdentityProfileService<TUser>>();
            services.AddTransient<IResourceOwnerPasswordValidator, IdentityResourceOwnerPasswordValidator<TUser>>();

            services.Configure<IdentityOptions>(options => {
                options.Cookies.ApplicationCookie.AuthenticationScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
                options.Cookies.ApplicationCookie.LoginPath = new PathString("/ui/login");
                options.Cookies.ApplicationCookie.LogoutPath = new PathString("/ui/logout");

                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            services.Configure<IdentityServerOptions>(
               options => {
                   options.AuthenticationOptions.AuthenticationScheme = IdentityServerConstants.DefaultCookieAuthenticationScheme;
               });

            return builder;
        }
    }
}
