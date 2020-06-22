using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.State
{
    public class NewProjectModel
    {
        public string root {get; set;}
        public IFormFile package { get; set; }
    }
}
