using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCMAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadRichMenuController : ControllerBase
    {
        // GET: api/<RichMenuController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RichMenuController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RichMenuController>
        [HttpPost]
        public IActionResult Post([FromBody] RichMenuModel value)
        {
            var result = "";

            return Ok(result);
        }

        
        // DELETE api/<RichMenuController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
