namespace WebApplication
{
    // Strongly typed classes for settings defined in appsettings.json to be deserialized to

    public class AppBundleZipPaths
    {
        public string CreateSVF { get; set; }
        public string CreateThumbnail { get; set; }
        public string ExtractParameters { get; set; }
    }

    public class DefaultProjectsConfiguration
    {
        public DefaultProjectConfiguration[] Projects { get; set; }
    }

    public class DefaultProjectConfiguration
    {
        public string Url { get; set; }
        public string TopLevelAssembly { get; set; }
    }
}
