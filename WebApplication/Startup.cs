using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using WebApplication.Definitions;
using WebApplication.Processing;
using WebApplication.Utilities;

namespace WebApplication
{
    public class TokenHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<TokenHandlerMiddleware> logger, UserResolver resolver)
        {
            if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var token))
            {
                logger.LogInformation($"Found token: {token}");
                resolver.Token = token;
            }
            else
            {
                logger.LogInformation("Cannot find token");
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    public class UserResolver
    {
        private readonly ResourceProvider _resourceProvider;
        private readonly IForgeOSS _forgeOSS;

        public string Token { private get; set; }
        public bool IsAuthenticated => ! string.IsNullOrEmpty(Token);

        public UserResolver(ResourceProvider resourceProvider, IForgeOSS forgeOSS)
        {
            _resourceProvider = resourceProvider;
            _forgeOSS = forgeOSS;
        }

        public async Task<string> GetBucketKey()
        {
            if (IsAuthenticated)
            {
                var profile = await _forgeOSS.GetProfileAsync(Token);
                var userId = profile.userId;

                var userHash = Crypto.GenerateHashString(userId);

                var bucketKey = $"authd-{userId.Substring(0, 3)}-{userHash}".ToLowerInvariant();
                await _forgeOSS.CreateBucketAsync(bucketKey); // TODO: can throw an exception?

                return bucketKey;
            }
            else
            {
                return _resourceProvider.BucketKey;
            }
        }
    }


    public class Startup
    {
        private const string ForgeSectionKey = "Forge";
        private const string AppBundleZipPathsKey = "AppBundleZipPaths";
        private const string DefaultProjectsSectionKey = "DefaultProjects";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddHttpClient();

            // NOTE: eventually we might want to use `AddForgeService()`, but right now it might break existing stuff
            // https://github.com/Autodesk-Forge/forge-api-dotnet-core/blob/master/src/Autodesk.Forge.Core/ServiceCollectionExtensions.cs
            services.Configure<ForgeConfiguration>(Configuration.GetSection(ForgeSectionKey));
            services.AddSingleton<ResourceProvider>();
            services.AddSingleton<IPostProcessing, PostProcessing>();
            services.AddSingleton<IForgeOSS, ForgeOSS>();
            services.Configure<AppBundleZipPaths>(Configuration.GetSection(AppBundleZipPathsKey));
            services.AddSingleton<FdaClient>();
            services.Configure<DefaultProjectsConfiguration>(Configuration.GetSection(DefaultProjectsSectionKey));
            services.AddTransient<Initializer>();
            services.AddTransient<Arranger>();
            services.AddTransient<ProjectWork>();
            services.AddTransient<DtoGenerator>();
            services.AddSingleton<DesignAutomationClient>(provider =>
                                    {
                                        var forge = provider.GetService<IForgeOSS>();
                                        var httpMessageHandler = new ForgeHandler(Options.Create(forge.Configuration))
                                        {
                                            InnerHandler = new HttpClientHandler()
                                        };
                                        var forgeService = new ForgeService(new HttpClient(httpMessageHandler));
                                        return new DesignAutomationClient(forgeService);
                                    });
            services.AddSingleton<Publisher>();
            services.AddScoped<UserResolver>(); // TODO: use interface
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Initializer initializer, ILogger<Startup> logger, ResourceProvider resourceProvider)
        {
            if(Configuration.GetValue<bool>("clear"))
            {
                logger.LogInformation("-- Clean up --");
                initializer.ClearAsync().Wait();
            }

            if(Configuration.GetValue<bool>("initialize"))
            {
                logger.LogInformation("-- Initialization --");
                initializer.InitializeAsync().Wait();
            }

            if (env.IsDevelopment())
            {
                logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<TokenHandlerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // expose local cache dir as 'data' virtual dir to serve locally cached OSS files
            Directory.CreateDirectory(resourceProvider.LocalRootName);
            app.UseStaticFiles(new StaticFileOptions
            {
                // make sure that directory exists
                FileProvider = new PhysicalFileProvider(resourceProvider.LocalRootName),
                RequestPath = new PathString(ResourceProvider.VirtualCacheDir),
                ServeUnknownFileTypes = true
            });

            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Controllers.JobsHub>("/signalr/connection");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
