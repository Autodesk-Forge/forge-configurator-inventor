namespace WebApplication.Definitions
{
    public class CallbackUrls
    {
        public string Base { get; set; }
        
        public MethodsConfiguration Methods { get; set; }
        
        public class MethodsConfiguration
        {
            public string Update { get; set; }
            public string Adopt { get; set; }
            public string GenSat { get; set; }
            public string GenRfa { get; set; }
        }
    }
}