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

        // generate unique names for files. The files will be moved to correct places after hash generation.
        public readonly string Parameters = $"{Guid.NewGuid():N}.json";
        public readonly string Thumbnail = $"{Guid.NewGuid():N}.png";
        public readonly string SVF = $"{Guid.NewGuid():N}.zip";

        /// <summary>
        /// Constructor.
        /// </summary>
        public Arranger(IForgeOSS forge, string bucketKey)
        {
            _forge = forge;
            _bucketKey = bucketKey;
        }

        /// <summary>
        /// Create adoption data.
        /// </summary>
        /// <param name="docUrl">URL to the input Inventor document (IPT or zipped IAM)</param>
        /// <param name="tlaFilename">Top level assembly in the ZIP. (if any)</param>
        public async Task<AdoptionData> ForAdoption(string docUrl, string tlaFilename)
        {
            return new AdoptionData // TODO: check - can URLs be generated in parallel?
            {
                InputUrl = docUrl,
                ThumbnailUrl = await _forge.CreateSignedUrlAsync(_bucketKey, Thumbnail, ObjectAccess.Write),
                SvfUrl = await _forge.CreateSignedUrlAsync(_bucketKey, SVF, ObjectAccess.Write),
                ParametersJsonUrl = await _forge.CreateSignedUrlAsync(_bucketKey, Parameters, ObjectAccess.Write),
                TLA = tlaFilename
            };
        }

        /// <summary>
        /// Move OSS objects to correct places.
        /// NOTE: it's expected that the data is generated already.
        /// </summary>
        public async Task Do(Project project)
        {
            using var client = new HttpClient(); // TODO: should we have cache for it?

            // rearrange generated data according to the parameters hash
            var url = await _forge.CreateSignedUrlAsync(_bucketKey, Parameters);
            using var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // generate hash for parameters
            Stream stream = await response.Content.ReadAsStreamAsync();
            var hashString = Crypto.GenerateStreamHashString(stream);

            // move data to expected places
            // TODO: check - can it be done in parallel?
            await _forge.RenameObjectAsync(_bucketKey, Thumbnail, project.Attributes.Thumbnail);

            var keyProvider = project.KeyProvider(hashString);
            await _forge.RenameObjectAsync(_bucketKey, SVF, keyProvider.ModelView);
            await _forge.RenameObjectAsync(_bucketKey, Parameters, keyProvider.Parameters);
        }
    }
}
