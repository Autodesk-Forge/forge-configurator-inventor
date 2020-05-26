namespace WebApplication.Definitions
{
    /// <summary>
    /// All data required for project adoption.
    /// </summary>
    public class AdoptionData
    {
        public string InputDocUrl { get; set; }
        public string InputParamsUrl { get; set; }

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
