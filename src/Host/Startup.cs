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
using RigoFunc.ApiCore;
using RigoFunc.ApiCore.Default;
using RigoFunc.ApiCore.Filters;
using RigoFunc.IdentityServer;

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

            // Add API invoker services
            services.AddApiInvoker(options => {
                options.SendSmsApi = Configuration["ApiUrls:Sms"];
                options.SendEmailApi = Configuration["ApiUrls:Email"];
                options.AppPushApi = Configuration["ApiUrls:AppPush"];
                options.HeaderRetriever = (url) => {
                    return new[] {
                        new Tuple<string, string>("xunit", "59d63571-c4c2-4daa-aac6-969f581dc1fa")
                    };
                };
            });

            // Use RigoFunc.Account default account service.
            services.AddAccountService<AppUser>(options => {
                options.DefaultClientId = "system";
                options.DefaultClientSecret = "secret";
                options.DefaultScope = "doctor consultant finance order payment";
                options.CodeSmsTemplate = "SMS_1101";
                options.PasswordSmsTemplate = "SMS_1101";
            });

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
                .AddCustomGrantValidator<CustomGrantValidator>()
                .AddDistributedStores()
                .ConfigureAspNetCoreIdentity<AppUser>()
                .AllowCors(options => {
                    options.AllowAnyOrigin = true;
                });

            services.AddScoped<IApiResultHandler, DefaultApiResultHandler>();
            services.AddScoped<IApiExceptionHandler, DefaultApiExceptionHandler>();

            // for the UI
            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                }).AddMvcOptions(options => {
                    options.Filters.Add(typeof(ApiResultFilterAttribute));
                    options.Filters.Add(typeof(ApiExceptionFilterAttribute));
                })
                .AddRazorOptions(razor => {
                    razor.ViewLocationExpanders.Add(new UI.CustomViewLocationExpander());
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