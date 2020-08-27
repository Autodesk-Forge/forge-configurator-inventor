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

using System.IO;
using System.Threading.Tasks;
using WebApplication.Definitions;
using Xunit;

namespace WebApplication.Tests
{
    // This initialization test cannot run in parallel because it touches the same files as other init test
    [Collection("IntegrationTests1")]
    public class NoDefaultProjectInitIntegrationTest : InitializerTestBase, IAsyncLifetime
    {
        private static readonly DefaultProjectsConfiguration defaultProjectsConfiguration = new DefaultProjectsConfiguration();

        public NoDefaultProjectInitIntegrationTest() : base(defaultProjectsConfiguration)
        {}

        public async Task DisposeAsync()
        {
            await initializer.ClearAsync(false);
        }

        public async Task InitializeAsync()
        {
            await initializer.ClearAsync(false);
        }

        [Fact]
        public async void NoDefaultProjectInitTestAsync()
        {
            // Init the project with no default project as this would previously fail on null reference exception
            // while iterating the default projects
            await initializer.InitializeAsync();
            // Secondary defect from the first one caused the local cache directory to be removed druing clear - init - run sequence in one go
            Assert.True(Directory.Exists(localCache.LocalRootName));
        }
    }
}
