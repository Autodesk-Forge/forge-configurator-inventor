using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebApplication.Utilities;

namespace WebApplication.Processing
{
    /// <summary>
    /// Class to place generated data files to expected place.
    /// </summary>
    public class Arranger
    {
        private readonly IForgeOSS _forge;
        private readonly string _bucketKey;
        private readonly IHttpClientFactory _clientFactory;

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
        }

        /// <summary>
        /// Create adoption data.
        /// </summary>
        /// <param name="docUrl">URL to the input Inventor document (IPT or zipped IAM)</param>
        /// <param name="tlaFilename">Top level assembly in the ZIP. (if any)</param>
        public async Task<AdoptionData> ForAdoption(string docUrl, string tlaFilename)
        {
            var urls = await Task.WhenAll(_forge.CreateSignedUrlAsync(_bucketKey, Thumbnail, ObjectAccess.Write), 
                                            _forge.CreateSignedUrlAsync(_bucketKey, SVF, ObjectAccess.Write), 
                                            _forge.CreateSignedUrlAsync(_bucketKey, Parameters, ObjectAccess.Write));

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
        public async Task Do(Project project)
        {
            var client = _clientFactory.CreateClient();

            // rearrange generated data according to the parameters hash
            var url = await _forge.CreateSignedUrlAsync(_bucketKey, Parameters);
            using var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // generate hash for parameters
            Stream stream = await response.Content.ReadAsStreamAsync();
            var hashString = Crypto.GenerateStreamHashString(stream);

            var keyProvider = project.KeyProvider(hashString);

            // move data to expected places
            await Task.WhenAll(_forge.RenameObjectAsync(_bucketKey, Thumbnail, project.Attributes.Thumbnail),
                                _forge.RenameObjectAsync(_bucketKey, SVF, keyProvider.ModelView),
                                _forge.RenameObjectAsync(_bucketKey, Parameters, keyProvider.Parameters));
        }
    }
}
