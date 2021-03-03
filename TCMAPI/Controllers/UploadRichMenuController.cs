using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using TCMAPI.Commands;
using TCMAPI.Models;

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadRichMenuController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        private IWebHostEnvironment _hostingEnvironment;
        public UploadRichMenuController(IOptions<AppSettingModel> app, IWebHostEnvironment environment)
        {
            appSettings = app;
            _hostingEnvironment = environment;
        }

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
        public IActionResult Post([FromBody] RootRichMenuModel val)
        {
            var menuId = "richmenu-fdd7c0f485c9f69d3fd09f54a6234fcb";
            var result = "";
            try
            {
                result = UploadImageRichMenu("", "");
                if (result != "Success")
                {
                    return NotFound(result);
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            
            return Ok(result);
        }

        protected string UploadRichMenu()
        {
            string result = "";

            return result;
        }
        public string UploadImageRichMenu(string menuId, string imgBase64)
        {
            string result = "Success";
            string AppSettingUrl = appSettings.Value.UploadImageUrl;
            string UploadImageUrl = String.Format(AppSettingUrl, menuId);
            string lineAccessToken = appSettings.Value.LineChannelAccessToken;
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
