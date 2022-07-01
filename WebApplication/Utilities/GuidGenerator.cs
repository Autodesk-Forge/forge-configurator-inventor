using System;

namespace WebApplication.Utilities
{
    public interface IGuidGenerator
    {
        string GenerateGuid();
    }
    
    public class GuidGenerator : IGuidGenerator
    {
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}