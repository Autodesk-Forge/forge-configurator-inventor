using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace IoConfigDemo.Controllers
{
    [Route("[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}
