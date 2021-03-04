using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using TCMAPI.Models;

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadRichMenuController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        private string lineAccessToken;
        public UploadRichMenuController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
            lineAccessToken = appSettings.Value.LineChannelAccessToken;
    }

        // GET: api/<RichMenuController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Test", "Success" };
        }

        // GET api/<RichMenuController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RichMenuController>
        [HttpPost]
        public IActionResult Post([FromBody] RichMenuModel_FromBackend val)
        {
          
            var result = "";
            var key = "";
            var menuId = "";
            var imageBase64 = val.img;
            try
            {
                var returnText = CreateRichMenu(val);
                
                dynamic value = JsonConvert.DeserializeObject(returnText);
                foreach (JProperty item in value)
                {
                    key = item.Name;
                    menuId = (string)item.Value;
                }

                if(key == "richMenuId")
                {
                    result = UploadImageRichMenu(menuId, imageBase64);
                    if (result != "Success")
                    {
                        return NotFound(result);
                    }
                }
                else
                {
                    return NotFound(menuId);
                }
                
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            
            return Ok(result);
        }

        protected string CreateRichMenu(RichMenuModel_FromBackend val)
        {
            string result = "";
            string CreateRichMenu = appSettings.Value.CreateRichMenuUrl;
            var client = new RestClient(CreateRichMenu);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            try
            {
                string output = JsonConvert.SerializeObject(val);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", lineAccessToken);
                request.AddParameter("application/json",output, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                result = response.Content;

                //HttpStatusCode statusCode = response.StatusCode;
                //if ((int)statusCode == 200)
                //{
                //    dynamic value = JsonConvert.DeserializeObject(response.Content);
                //    foreach(JProperty item in value)
                //    {
                //        var key = item.Name;
                //    }
                //    result = value.richMenuId;
                //}
                //else
                //{
                //    result = response.Content;
                //}
                
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
        protected string UploadImageRichMenu(string menuId, string imgBase64)
        {
            string result = "Success";
            string AppSettingUrl = appSettings.Value.UploadImageUrl;
            string UploadImageUrl = String.Format(AppSettingUrl, menuId);
            var client = new RestClient(UploadImageUrl);
            var request = new RestRequest(Method.POST);
            byte[] bytes = System.Convert.FromBase64String(imgBase64);

            try
            {
                request.AddHeader("Content-Type", "image/jpeg");
                request.AddHeader("Authorization", lineAccessToken);
                request.AddParameter("image/jpeg", bytes, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
