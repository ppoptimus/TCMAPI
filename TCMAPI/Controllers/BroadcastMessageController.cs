using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TCMAPI.Models;


namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BroadcastMessageController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public BroadcastMessageController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        // GET: api/<BroadcastMessageController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "status", "connected" };
        }

        
        // POST api/BroadcastMessage
        [HttpPost]
        public IActionResult Post([FromBody] BroadcastMessageModel val)
        {
            string result = "Success";
            var messge = val.messages.Select(x => x.text).First();
            try
            {
                string BroadcastMessage = appSettings.Value.BroadcastMessageUrl;
                var client = new RestClient(BroadcastMessage);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n    \"messages\": [\r\n        {\r\n            \"type\":\"text\",\r\n            \"text\":\""+ messge + "\"\r\n        }\r\n    ]\r\n}", ParameterType.RequestBody);
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
                return NotFound(result);
            }

            return Ok(result);
        }

    }
}
