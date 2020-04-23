namespace WebApplication.Definitions
{
    public class ProjectDTO
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Image { get; set; }

        /// <summary>
        /// Parameters hash.
        /// </summary>
        public string Hash { get; set; }
    }
}
