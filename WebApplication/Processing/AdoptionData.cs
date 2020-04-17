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
        private readonly IForge _forge;
        private readonly string _bucketKey;

        // generate unique names for files. The files will be moved to correct places after hash generation.
        public readonly string Parameters = $"{Guid.NewGuid():N}.json";
        public readonly string Thumbnail = $"{Guid.NewGuid():N}.png";
        public readonly string SVF = $"{Guid.NewGuid():N}.zip";

        public Arranger(IForge forge, string bucketKey)
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
                        ThumbnailUrl = await _forge.CreateSignedUrl(_bucketKey, Thumbnail, ObjectAccess.Write),
                        SvfUrl = await _forge.CreateSignedUrl(_bucketKey, SVF, ObjectAccess.Write),
                        ParametersJsonUrl = await _forge.CreateSignedUrl(_bucketKey, Parameters, ObjectAccess.Write),
                        TLA = tlaFilename
                    };
        }

        /// <summary>
        /// Move OSS objects to correct places.
        /// NOTE: it's expected that the data is generated already.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task Do(Project project)
        {
            using var client = new HttpClient(); // TODO: should we have cache for it?

            // rearrange generated data according to the parameters hash
            var url = await _forge.CreateSignedUrl(_bucketKey, Parameters);
            using var response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            // generate hash for parameters
            Stream stream = await response.Content.ReadAsStreamAsync();
            var hashString = Crypto.GenerateStreamHashString(stream);

            // move data to expected places
            var keyProvider = project.KeyProvider(hashString);
            await Move(keyProvider);
        }

        private async Task Move(OSSObjectKeyProvider keyProvider)
        {
            // TODO: check - can it be done in parallel?
            //await _forge.RenameObject(_bucketKey, Thumbnail, keyProvider.); // TODO: handle thumbnail location
            await _forge.RenameObject(_bucketKey, SVF, keyProvider.ModelView);
            await _forge.RenameObject(_bucketKey, Parameters, keyProvider.Parameters);
        }
    }

    /// <summary>
    /// All data required for project adoption.
    /// </summary>
    public class AdoptionData
    {
        public string InputUrl { get; set; }

        /// <summary>
        /// Relative path to top level assembly in ZIP with assembly.
        /// </summary>
        public string TLA { get; set; }

        public string ThumbnailUrl { get; set; }
        public string SvfUrl { get; set; }
        public string ParametersJsonUrl { get; set; }

        /// <summary>
        /// If job data contains assembly.
        /// </summary>
        public bool IsAssembly => ! string.IsNullOrEmpty(TLA);
    }
}
