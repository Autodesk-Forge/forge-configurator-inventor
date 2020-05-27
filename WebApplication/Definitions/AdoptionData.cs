namespace WebApplication.Definitions
{
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

        public string ModelUrl { get; set; } // TODO: temporary!
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
//        public string ModelUrl { get; set; }
    }
}
