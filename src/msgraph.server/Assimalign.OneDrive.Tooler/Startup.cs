using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace Assimalign.OneDrive.Tooler
{
    using Assimalign.OneDrive.Tooler.Options;
    using Assimalign.OneDrive.Tooler.Attributes;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.FileProviders;

    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<GraphConfigurations>()
                .Configure<IConfiguration>((settings, configure) =>
                {
                    configure.GetSection("Graph").Bind(settings);
                });

            services.Configure<FormOptions>(options =>
            {
                
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.ValueCountLimit = 10000; //default 1024
                options.ValueLengthLimit = int.MaxValue; //not recommended value
                options.MultipartBodyLengthLimit = long.MaxValue; //not recommended value
            });

            services.AddOptions<DriveStructure>()
                .Configure<IConfiguration>((settings, configure) =>
                {
                    configure.GetSection("DriveStructure").Bind(settings);
                });

            services.AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure(options =>
                {
                    options.TokenValidationParameters.RoleClaimType = "roles";
                    options.GetClaimsFromUserInfoEndpoint = true;
                });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddMicrosoftGraphAppOnly(provider => new GraphServiceClient(GraphClientFactory.Create(provider)))
                .AddInMemoryTokenCaches();

            //services.AddAuthorization(configure =>
            //{
            //    configure.AddPolicy(AuthorizationPolicy.AdminAuthPolicy, policy => policy.RequireRole(AuthorizationRole.AppManagementRole));
            //});


            services.AddControllers(options=>
            {
                options.Filters.Add(new DisableFormValueModelBindingAttribute());
                //options.Filters.Add(new GenerateAntiforgeryCookieAttribute());

            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // To list physical files from a path provided by configuration:
            var physicalProvider = new PhysicalFileProvider(Configuration.GetValue<string>("StoredFilesPath"));
           

            // To list physical files in the temporary files folder, use:
            //var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());

            services.AddSingleton<IFileProvider>(physicalProvider);



            services.AddCors(policy => policy.AddDefaultPolicy(
                configure =>
                {
                    configure.AllowAnyOrigin();
                    configure.AllowAnyMethod();
                    configure.AllowAnyHeader();
                }));

            services.AddScoped<IGraphServiceClient, GraphServiceClient>(serviceProvider => 
                serviceProvider.GetRequiredService<GraphServiceClient>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
