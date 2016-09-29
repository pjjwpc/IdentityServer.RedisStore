using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Host.Configuration;
using Host.Cors;
using Host.EntityFrameworkCore;
using Host.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Host {
    public class Startup {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env) {
            _environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(Configuration["Data:Default:ConnectionString"], b => b.MigrationsAssembly("Host")));

            services.AddIdentity<AppUser, IdentityRole<Guid>>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext, Guid>()
            .AddDefaultTokenProviders();

            services.AddDistributedSqlServerCache(options => {
                options.ConnectionString = Configuration["Data:Default:ConnectionString"];
                options.SchemaName = "dbo";
                options.TableName = "TokenCache";
            });

            var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "idsrv3test.pfx"), "idsrv3test");
            var builder = services.AddIdentityServer(options => {
                options.UserInteractionOptions.LoginUrl = "/ui/login";
                options.UserInteractionOptions.LogoutUrl = "/ui/logout";
                options.UserInteractionOptions.ConsentUrl = "/ui/consent";
                options.UserInteractionOptions.ErrorUrl = "/ui/error";
            })
                .SetSigningCredential(cert)
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryScopes(Scopes.Get())
                .AllowCors(options => {
                    options.AllowAnyOrigin = true;
                });

            //builder.AddExtensionGrantValidator<Extensions.ExtensionGrantValidator>();

            // for the UI
            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });

        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            //loggerFactory.AddConsole((scope, level) => scope.StartsWith("IdentityServer"));
            //loggerFactory.AddDebug((scope, level) => scope.StartsWith("IdentityServer"));
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseIdentity();
            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}
