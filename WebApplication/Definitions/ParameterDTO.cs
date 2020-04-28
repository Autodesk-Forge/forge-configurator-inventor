namespace WebApplication.Definitions
{
    public class ParameterDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; } // TODO: make it enum
        public string Units { get; set; }
        public string[] AllowedValues { get; set; }
    }

    public class ParametersDTO
    {
        public ParameterDTO[] Parameters { get; set; }
    }
}
