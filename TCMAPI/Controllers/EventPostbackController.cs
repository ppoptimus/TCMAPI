using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TCMAPI.Models;

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventPostbackController : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody] RootEventMessage val)
        {
            var userId = val.events.Select(x => x.source.userId).FirstOrDefault().ToString();
        }
    }
}
