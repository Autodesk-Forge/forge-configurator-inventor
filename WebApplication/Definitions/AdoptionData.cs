namespace WebApplication.Definitions
{
    //TODO: split the update urls and rfa urls
    public class ProcessingArgs
    {
        public string InputDocUrl { get; set; }

        /// <summary>
        /// Relative path to top level assembly in ZIP with assembly.
        /// </summary>
        public string TLA { get; set; }

        public string SvfUrl { get; set; }
        public string ParametersJsonUrl { get; set; }

        /// <summary>
        /// If job data contains assembly.
        /// </summary>
        public bool IsAssembly => ! string.IsNullOrEmpty(TLA);

        public string OutputModelUrl { get; set; } // TODO: temporary!
        public string SatUrl { get; internal set; }
        public string RfaUrl { get; internal set; }
    }

    /// <summary>
    /// All data required for project adoption.
    /// </summary>
    public class AdoptionData : ProcessingArgs
    {
        public string ThumbnailUrl { get; set; }
    }

    /// <summary>
    /// All data required for project update.
    /// </summary>
    public class UpdateData : ProcessingArgs
    {
        public string InputParamsUrl { get; set; }
//        public string OutputModelUrl { get; set; }
    }
}
