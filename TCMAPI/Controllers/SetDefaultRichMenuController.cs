using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    public class SetDefaultRichMenuController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public SetDefaultRichMenuController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string val)
        {
            var result = "";
            result = SetDefaultMenu(val);
            return Ok(result);
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
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
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
