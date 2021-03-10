using Microsoft.AspNetCore.Mvc;
using NLog;
using System;

namespace TCMAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        
        [HttpGet]
        public IActionResult Get()
        {
            _logger.Info(DateTime.Now.ToString());
            return Ok("Connected");
        }
    }
}
