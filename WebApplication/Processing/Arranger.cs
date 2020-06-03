using System;
using System.Net.Http;
using System.Text.Json;
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
        public readonly string InputParams = $"{Guid.NewGuid():N}.json";
        public readonly string OutputModel = $"{Guid.NewGuid():N}.zip";
        public readonly string OutputSAT = $"{Guid.NewGuid():N}.sat";
        public readonly string OutputRFA = $"{Guid.NewGuid():N}.rfa";

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
            var urls = await Task.WhenAll(CreateSignedUrlAsync(Thumbnail, ObjectAccess.Write), 
                                            CreateSignedUrlAsync(SVF, ObjectAccess.Write), 
                                            CreateSignedUrlAsync(Parameters, ObjectAccess.Write),
                                            CreateSignedUrlAsync(OutputModel, ObjectAccess.Write));

            return new AdoptionData
            {
                InputDocUrl       = docUrl,
                ThumbnailUrl      = urls[0],
                SvfUrl            = urls[1],
                ParametersJsonUrl = urls[2],
                OutputModelUrl    = urls[3],
                TLA               = tlaFilename
            };
        }

        /// <summary>
        /// Create adoption data.
        /// </summary>
        /// <param name="docUrl">URL to the input Inventor document (IPT or zipped IAM)</param>
        /// <param name="tlaFilename">Top level assembly in the ZIP. (if any)</param>
        /// <param name="parameters">Inventor parameters.</param>
        public async Task<UpdateData> ForUpdateAsync(string docUrl, string tlaFilename, InventorParameters parameters)
        {
            var urls = await Task.WhenAll(
                                            CreateSignedUrlAsync(OutputModel, ObjectAccess.Write),
                                            CreateSignedUrlAsync(SVF, ObjectAccess.Write),
                                            CreateSignedUrlAsync(Parameters, ObjectAccess.Write),
                                            CreateSignedUrlAsync(InputParams, ObjectAccess.ReadWrite));

            await using var jsonStream = Json.ToStream(parameters);
            await _forge.UploadObjectAsync(_resourceProvider.BucketKey, InputParams, jsonStream);

            return new UpdateData
            {
                InputDocUrl       = docUrl,
                OutputModelUrl    = urls[0],
                SvfUrl            = urls[1],
                ParametersJsonUrl = urls[2],
                InputParamsUrl    = urls[3],
                TLA               = tlaFilename
            };
        }

        /// <summary>
        /// Move project OSS objects to correct places.
        /// NOTE: it's expected that the data is generated already.
        /// </summary>
        /// <returns>Parameters hash.</returns>
        public async Task<string> MoveProjectAsync(Project project, string tlaFilename)
        {
            var hashString = await GenerateParametersHashAsync();
            var attributes = new ProjectMetadata { Hash = hashString, TLA = tlaFilename };

            var ossNames = project.OssNameProvider(hashString);

            // move data to expected places
            await Task.WhenAll(_forge.RenameObjectAsync(_bucketKey, Thumbnail, project.OssAttributes.Thumbnail),
                                _forge.RenameObjectAsync(_bucketKey, SVF, ossNames.ModelView),
                                _forge.RenameObjectAsync(_bucketKey, Parameters, ossNames.Parameters),
                                _forge.RenameObjectAsync(_bucketKey, OutputModel, ossNames.CurrentModel),
                                _forge.UploadObjectAsync(_bucketKey, project.OssAttributes.Metadata, Json.ToStream(attributes, writeIndented: true)));

            return hashString;
        }

        internal async Task<string> MoveRfaAsync(Project project, string hash)
        {
            var ossNames = project.OssNameProvider(hash);
            await Task.WhenAll(_forge.RenameObjectAsync(_bucketKey, OutputRFA, ossNames.Rfa),
                                _forge.DeleteAsync(_bucketKey, OutputSAT));
            return await _forge.CreateSignedUrlAsync(_bucketKey, ossNames.Rfa);
        }

        internal async Task<ProcessingArgs> ForRfaAsync(string inputDocUrl, string topLevelAssembly)
        {
            var urls = await Task.WhenAll(  CreateSignedUrlAsync(OutputSAT, ObjectAccess.ReadWrite),
                                            CreateSignedUrlAsync(OutputRFA, ObjectAccess.Write));

            return new ProcessingArgs
            {
                InputDocUrl = inputDocUrl,
                OutputModelUrl = null,
                SvfUrl = null,
                ParametersJsonUrl = null,
                TLA = topLevelAssembly,
                SatUrl = urls[0],
                RfaUrl = urls[1]
            };
        }

        /// <summary>
        /// Move viewables OSS objects to correct places.
        /// NOTE: it's expected that the data is generated already.
        /// </summary>
        /// <returns>Parameters hash.</returns>
        public async Task<string> MoveViewablesAsync(Project project)
        {
            var hashString = await GenerateParametersHashAsync();

            var ossNames = project.OssNameProvider(hashString);

            // move data to expected places
            await Task.WhenAll(_forge.RenameObjectAsync(_bucketKey, SVF, ossNames.ModelView),
                                _forge.RenameObjectAsync(_bucketKey, Parameters, ossNames.Parameters),
                                _forge.RenameObjectAsync(_bucketKey, OutputModel, ossNames.CurrentModel),
                                _forge.DeleteAsync(_bucketKey, InputParams));

            return hashString;
        }

        /// <summary>
        /// Generate hash string for the _temporary_ parameters json.
        /// </summary>
        private async Task<string> GenerateParametersHashAsync()
        {
            var client = _clientFactory.CreateClient();

            // rearrange generated data according to the parameters hash
            var url = await CreateSignedUrlAsync(Parameters);
            using var response = await client.GetAsync(url); // TODO: find
            response.EnsureSuccessStatusCode();

            // generate hash for parameters
            var stream = await response.Content.ReadAsStreamAsync();
            var parameters = await JsonSerializer.DeserializeAsync<InventorParameters>(stream);
            return Crypto.GenerateObjectHashString(parameters);
        }

        private Task<string> CreateSignedUrlAsync(string objectName, ObjectAccess access = ObjectAccess.Read)
        {
            return _forge.CreateSignedUrlAsync(_bucketKey, objectName, access);
        }
    }
}
