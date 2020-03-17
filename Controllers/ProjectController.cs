using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IoConfigDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectController : ControllerBase
    {
        private static readonly string[] Names = new[]
        {
            "Project1", "Project2", "Project3", "Project4", "Project5", "Project6", "Project7", "Project8", "Project9", "Project10"
        };

        private readonly ILogger<ProjectController> _logger;

        public ProjectController(ILogger<ProjectController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Project> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Project
            {
                Date = DateTime.Now.AddDays(index),
                Name = Names[rng.Next(Names.Length)]
            })
            .ToArray();
        }
    }
}
