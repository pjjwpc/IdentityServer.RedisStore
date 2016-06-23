using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RigoFunc.Account;
using RigoFunc.Account.Services;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/> for configuring identity services.
    /// </summary>
    public static class IServiceCollectionExtensions {
        /// <summary>
        /// Configures the account API.
        /// </summary>
        /// <typeparam name="TUser">The type representing a User in the system.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="ApiOptions"/>.</param>
        /// <returns>The services available in the application.</returns>
        public static IServiceCollection ConfigureAccountApi<TUser>(this IServiceCollection services, Action<ApiOptions> setupAction)
            where TUser : class, new() {
            if (setupAction != null) {
                services.Configure(setupAction);
            }
            
            // try add default account service, or use the end-user DI in startup.cs
            services.TryAddTransient<IAccountService, DefaultAccountService<TUser>>();

            return services;
        }
    }
}
