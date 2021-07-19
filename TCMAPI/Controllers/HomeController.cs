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
        public ContentResult Get()
        {
            _logger.Info(DateTime.Now.ToString());

            return new ContentResult
            {
                ContentType = "text/html",
                Content = "<body style=\"background-color:cornflowerblue; \"><div style=\"text-align:center; \"><h1>Web service connect successful</h1></div></body>"
            };
        }
    }
}
