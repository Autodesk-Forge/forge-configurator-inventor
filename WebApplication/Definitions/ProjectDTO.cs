namespace WebApplication.Definitions
{
    public class ProjectDTO
    {
        public string Id { get; set; }
        public string Label { get; set; }

        /// <summary>
        /// Thumbnail URL.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// URL to SVF directory.
        /// </summary>
        public string Svf { get; set; }
    }
}
