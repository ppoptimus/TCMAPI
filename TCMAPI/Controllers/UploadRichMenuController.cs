using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
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

        // POST api/<RichMenuController>
        [HttpPost]
        public IActionResult Post([FromBody] RichMenuModel_FromBackend val)
        {
            bool isSetDefault = false;
            var result = "";
            var resultFromUploadImage = "";
            var resultFromSetDeaultMenu = "";
            var key = "";
            var menuId = "";
            var imageBase64 = val.img;
            isSetDefault = val.setDefault;
            try
            {
                result = CreateRichMenu(val);
                
                dynamic value = JsonConvert.DeserializeObject(result);
                foreach (JProperty item in value)
                {
                    key = item.Name;
                    menuId = (string)item.Value;
                }
                
                if (key == "richMenuId")
                {
                    resultFromUploadImage = UploadImageRichMenu(menuId, imageBase64);
                    resultFromSetDeaultMenu = (isSetDefault) ? SetDefaultMenu(menuId) : "Success";
                    if (resultFromUploadImage != "Success" || resultFromSetDeaultMenu != "Success")
                    {
                        return NotFound(string.Join(resultFromUploadImage, " ", resultFromSetDeaultMenu));
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
            string appSettingUrl = appSettings.Value.UploadImageUrl;
            string uploadImageUrl = String.Format(appSettingUrl, menuId);
            var client = new RestClient(uploadImageUrl);
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
        protected string SetDefaultMenu(string menuId)
        {
            string result = "Success";
            string appSettingUrl = appSettings.Value.SetDefaultMenu;
            string setDefaultMenu = String.Format(appSettingUrl, menuId);
            var client = new RestClient(setDefaultMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Authorization", lineAccessToken);
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
            return result;
        }
    }
}
