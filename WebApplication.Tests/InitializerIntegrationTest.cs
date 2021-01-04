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

using Autodesk.Forge.Model;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication.Definitions;
using Xunit;
using Project = WebApplication.State.Project;

namespace WebApplication.Tests
{
    [Collection("IntegrationTests1")]
    public class InitializerIntegrationTest : InitializerTestBase, IAsyncLifetime
    {
        const string testZippedIamUrl = "http://testipt.s3-us-west-2.amazonaws.com/Basic.zip";
        const string testIamPathInZip = "iLogicBasic1.iam";

        private static readonly DefaultProjectsConfiguration defaultProjectsConfiguration = new DefaultProjectsConfiguration
        {
            Projects = new[] { new DefaultProjectConfiguration { Url = testZippedIamUrl, TopLevelAssembly = testIamPathInZip, Name = "Basic" } }
        };

        public InitializerIntegrationTest() : base(defaultProjectsConfiguration)
        { }

        public async Task InitializeAsync()
        {
            await initializer.ClearAsync(false);
        }

        public async Task DisposeAsync()
        {
            testFileDirectory.Delete(true);
            httpClient.Dispose();
            await initializer.ClearAsync(false);
        }

        private async Task<string> DownloadTestComparisonFile(string url, string name)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            string path = Path.Combine(testFileDirectory.FullName, name);
            using (var fs = new FileStream(path, FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }
            return path;
        }

        private async Task CompareOutputFileBytes(string expectedResultFileName, string outputFileUrl)
        {
            // download result, compare it to expected value
            byte[] expectedBytes = await File.ReadAllBytesAsync(expectedResultFileName);
            byte[] generatedOutputFileBytes = await httpClient.GetByteArrayAsync(outputFileUrl);

            // first confirm they are the same number of bytes
            Assert.Equal(expectedBytes.Length, generatedOutputFileBytes.Length);

            // then confirm each byte is the same
            for (int i = 0; i < expectedBytes.Length; i++)
            {
                Assert.True(expectedBytes[i] == generatedOutputFileBytes[i], $"unequal bytes at index {i}");
            }
        }

        [Fact]
        public async Task InitializeTestAsync()
        {
            await initializer.InitializeAsync();

            var project = new Project("Basic", Path.GetTempPath());

            // check thumbnail generated
            List<ObjectDetails> objects = await forgeOSS.GetBucketObjectsAsync(projectsBucketKey, project.OssAttributes.Thumbnail);
            Assert.Single(objects);
            string signedOssUrl = await forgeOSS.CreateSignedUrlAsync(projectsBucketKey, objects[0].ObjectKey);
            string testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/iLogicBasic1IamThumbnail.png", "iLogicBasic1IamThumbnail.png");
            await CompareOutputFileBytes(testComparisonFilePath, signedOssUrl);

            // check parameters generated with hashed name
            var ossNames = project.OssNameProvider("C93A317155D203B2C67D000446B32FAC8DF0B5F0");
            objects = await forgeOSS.GetBucketObjectsAsync(projectsBucketKey, ossNames.Parameters);
            Assert.Single(objects);
            signedOssUrl = await forgeOSS.CreateSignedUrlAsync(projectsBucketKey, objects[0].ObjectKey);
            testComparisonFilePath = await DownloadTestComparisonFile("http://testipt.s3-us-west-2.amazonaws.com/iLogicBasic1IamDocumentParams.json", "iLogicBasic1IamDocumentParams.json");
            await CompareOutputFileBytes(testComparisonFilePath, signedOssUrl);

            // check model view generated with hashed name (zip of SVF size/content varies slightly each time so we can only check if it was created)
            objects = await forgeOSS.GetBucketObjectsAsync(projectsBucketKey, ossNames.ModelView);
            Assert.Single(objects);
        }
    }
}
