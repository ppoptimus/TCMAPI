using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("Connected");
        }
    }
}
