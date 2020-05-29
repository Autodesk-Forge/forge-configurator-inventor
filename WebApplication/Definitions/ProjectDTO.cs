namespace WebApplication.Definitions
{
    public class ProjectDTO : ProjectDTOBase
    {
        public string Id { get; set; }
        public string Label { get; set; }

        /// <summary>
        /// Thumbnail URL.
        /// </summary>
        public string Image { get; set; }
    }
}
