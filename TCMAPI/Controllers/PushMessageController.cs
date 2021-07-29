using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TCMAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushMessageController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public PushMessageController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        // GET: api/<PushMessageController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Connected to TCM");
        }

        // POST api/PushMessage
        [HttpPost]
        public IActionResult Post([FromBody] PushMessageModel val)
        {
            string result = "success";
            try
            {
                var client = new RestClient(appSettings.Value.PushMessageUrl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                var body = JsonConvert.SerializeObject(val);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                HttpStatusCode statusCode = response.StatusCode;
                if ((int)statusCode != 200)
                {
                    result = response.Content;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
           
            return Ok(result);
        }

    }
}
