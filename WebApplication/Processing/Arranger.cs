using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    /// <summary>
    /// Class to place generated data files to expected places.
    /// </summary>
    public class Arranger
    {
        private readonly IForgeOSS _forge;
        private readonly string _bucketKey;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ResourceProvider _resourceProvider;

        // generate unique names for files. The files will be moved to correct places after hash generation.
        public readonly string Parameters = $"{Guid.NewGuid():N}.json";
        public readonly string Thumbnail = $"{Guid.NewGuid():N}.png";
        public readonly string SVF = $"{Guid.NewGuid():N}.zip";

        /// <summary>
        /// Constructor.
        /// </summary>
        public Arranger(IForgeOSS forge, IHttpClientFactory clientFactory, ResourceProvider resourceProvider)
        {
            _forge = forge;
            _bucketKey = resourceProvider.BucketKey;
            _clientFactory = clientFactory;
            _resourceProvider = resourceProvider;
        }

        /// <summary>
        /// Create adoption data.
        /// </summary>
        /// <param name="docUrl">URL to the input Inventor document (IPT or zipped IAM)</param>
        /// <param name="tlaFilename">Top level assembly in the ZIP. (if any)</param>
        public async Task<AdoptionData> ForAdoptionAsync(string docUrl, string tlaFilename)
        {
            var urls = await Task.WhenAll(_resourceProvider.CreateSignedUrlAsync(Thumbnail, ObjectAccess.Write), 
                                            _resourceProvider.CreateSignedUrlAsync(SVF, ObjectAccess.Write), 
                                            _resourceProvider.CreateSignedUrlAsync(Parameters, ObjectAccess.Write));

            return new AdoptionData
            {
                InputUrl          = docUrl,
                ThumbnailUrl      = urls[0],
                SvfUrl            = urls[1],
                ParametersJsonUrl = urls[2],
                TLA               = tlaFilename
            };
        }

        /// <summary>
        /// Move OSS objects to correct places.
        /// NOTE: it's expected that the data is generated already.
        /// </summary>
        public async Task DoAsync(Project project, string tlaFilename)
        {
            var hashString = await GenerateParametersHashAsync();
            var attributes = new ProjectMetadata { Hash = hashString, TLA = tlaFilename };

            var ossNames = project.OssNameProvider(hashString);

            // move data to expected places
            await Task.WhenAll(_forge.RenameObjectAsync(_bucketKey, Thumbnail, project.OssAttributes.Thumbnail),
                                _forge.RenameObjectAsync(_bucketKey, SVF, ossNames.ModelView),
                                _forge.RenameObjectAsync(_bucketKey, Parameters, ossNames.Parameters),
                                _forge.UploadObjectAsync(_bucketKey, Json.ToStream(attributes), project.OssAttributes.Metadata));
        }


        /// <summary>
        /// Generate hash string for the _temporary_ parameters json.
        /// </summary>
        private async Task<string> GenerateParametersHashAsync()
        {
            var client = _clientFactory.CreateClient();

            // rearrange generated data according to the parameters hash
            var url = await _resourceProvider.CreateSignedUrlAsync(Parameters);
            using var response = await client.GetAsync(url); // TODO: find
            response.EnsureSuccessStatusCode();

            // generate hash for parameters
            var stream = await response.Content.ReadAsStreamAsync();
            return Crypto.GenerateStreamHashString(stream);
        }
    }
}
