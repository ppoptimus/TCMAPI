using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TCMAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteMenuController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public DeleteMenuController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        // POST api/<DeleteMenuController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

    }
}
