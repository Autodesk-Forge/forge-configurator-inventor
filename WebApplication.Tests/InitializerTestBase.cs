/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Design Automation team for Inventor
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using WebApplication.Definitions;
using WebApplication.Middleware;
using WebApplication.Processing;
using WebApplication.Services;
using WebApplication.State;
using WebApplication.Utilities;

namespace WebApplication.Tests
{
    // Base class to setup Initializer class for other tests
    public abstract class InitializerTestBase
    {
        protected readonly ForgeOSS forgeOSS;
        protected readonly string projectsBucketKey;
        protected readonly Initializer initializer;
        protected readonly DirectoryInfo testFileDirectory;
        protected readonly HttpClient httpClient;
        protected readonly LocalCache localCache;

        public InitializerTestBase(DefaultProjectsConfiguration defaultProjectsConfiguration)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddForgeAlternativeEnvironmentVariables()
                .Build();

            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();

            ForgeConfiguration forgeConfiguration = configuration.GetSection("Forge").Get<ForgeConfiguration>();
            IOptions<ForgeConfiguration> forgeConfigOptions = Options.Create(forgeConfiguration);

            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            forgeOSS = new ForgeOSS(httpClientFactory, forgeConfigOptions, new NullLogger<ForgeOSS>());

            var httpMessageHandler = new ForgeHandler(Options.Create(forgeConfiguration))
            {
                InnerHandler = new HttpClientHandler()
            };
            var forgeService = new ForgeService(new HttpClient(httpMessageHandler));
            var designAutomationClient = new DesignAutomationClient(forgeService);

            projectsBucketKey = Guid.NewGuid().ToString();

            localCache = new LocalCache();
            var resourceProvider = new ResourceProvider(forgeConfigOptions, designAutomationClient, null, projectsBucketKey);
            var postProcessing = new PostProcessing(httpClientFactory, new NullLogger<PostProcessing>(), localCache, Options.Create(new ProcessingOptions()));
            var publisher = new Publisher(designAutomationClient, new NullLogger<Publisher>(), resourceProvider, postProcessing);

            var appBundleZipPathsConfiguration = new AppBundleZipPaths
            {
                EmptyExe = "../../../../WebApplication/AppBundles/EmptyExePlugin.bundle.zip",
                CreateSVF = "../../../../WebApplication/AppBundles/CreateSVFPlugin.bundle.zip",
                CreateThumbnail = "../../../../WebApplication/AppBundles/CreateThumbnailPlugin.bundle.zip",
                ExtractParameters = "../../../../WebApplication/AppBundles/ExtractParametersPlugin.bundle.zip",
                UpdateParameters = "../../../../WebApplication/AppBundles/UpdateParametersPlugin.bundle.zip",
                CreateSAT = "../../../../WebApplication/AppBundles/SatExportPlugin.bundle.zip",
                CreateRFA = "../../../../WebApplication/AppBundles/RFAExportPlugin.bundle.zip",
                CreateBOM = "../../../../WebApplication/AppBundles/ExportBOMPlugin.bundle.zip",
                ExportDrawing = "../../../../WebApplication/AppBundles/ExportDrawingAsPdfPlugin.bundle.zip",
                UpdateDrawings = "../../../../WebApplication/AppBundles/UpdateDrawingsPlugin.bundle.zip"
            };
            IOptions<AppBundleZipPaths> appBundleZipPathsOptions = Options.Create(appBundleZipPathsConfiguration);

            var fdaClient = new FdaClient(publisher, appBundleZipPathsOptions);
            IOptions<DefaultProjectsConfiguration> defaultProjectsOptions = Options.Create(defaultProjectsConfiguration);
            var userResolver = new UserResolver(resourceProvider, forgeOSS, forgeConfigOptions, localCache, NullLogger<UserResolver>.Instance, null);
            var arranger = new Arranger(httpClientFactory, userResolver);

            // TODO: linkGenerator should be mocked
            var dtoGenerator = new DtoGenerator(linkGenerator: null, localCache);
            var projectWork = new ProjectWork(new NullLogger<ProjectWork>(), arranger, fdaClient, dtoGenerator, userResolver);

            var projectService = new ProjectService(new NullLogger<ProjectService>(), userResolver, projectWork);

            initializer = new Initializer(new NullLogger<Initializer>(), fdaClient,
                                            defaultProjectsOptions, projectWork, userResolver, localCache,
                                            projectService);

            testFileDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            httpClient = new HttpClient();
        }
    }
}
