namespace WebApplication.Definitions
{
    // Strongly typed classes for settings defined in appsettings.json to be deserialized to

    public class AppBundleZipPaths
    {
        public string EmptyExe { get; set; }
        public string CreateSVF { get; set; }
        public string CreateThumbnail { get; set; }
        public string ExtractParameters { get; set; }
        public string UpdateParameters { get; set; }
        public string CreateSAT { get; set; }
        public string CreateRFA { get; set; }
    }

    public class DefaultProjectsConfiguration
    {
        public DefaultProjectConfiguration[] Projects { get; set; }
    }

    public class DefaultProjectConfiguration : ProjectInfo
    {
        public string Url { get; set; }
    }

    public class InviteOnlyModeConfiguration
    {
        public bool Enabled { get; set; }
        public string[] Domains { get; set; }
        public string[] Addresses { get; set; }
    }
}
