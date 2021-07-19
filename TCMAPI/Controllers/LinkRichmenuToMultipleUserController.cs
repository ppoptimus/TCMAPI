using Microsoft.AspNetCore.Http;
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

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkRichmenuToMultipleUserController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public LinkRichmenuToMultipleUserController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LinkMenuToMultipleModel val)
        {
            var result = "Success";
            if (ModelState.IsValid)
            {
                string json = JsonConvert.SerializeObject(val);
                result = LinkMenuToMultiple(json);
                if(result != "Success")
                {
                    return BadRequest(result);
                }
            }
            
            return Ok(result);
        }

        protected string LinkMenuToMultiple(string json)
        {
            string result = "Success";
            try
            {
                
                var client = new RestClient(appSettings.Value.LinkRichmenuToMultipleUser);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                HttpStatusCode statusCode = response.StatusCode;
                if ((int)statusCode != 202)
                {
                    result = response.Content;
                }
                
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
