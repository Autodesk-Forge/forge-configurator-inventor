namespace WebApplication.Definitions
{
    public class CallbackUrls
    {
        public bool UseCallbacks { get; set; }
        
        public string Base { get; set; }
        
        public MethodsConfiguration Methods { get; set; }
        
        public class MethodsConfiguration
        {
            public string Update { get; set; }
        }
    }
}