using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Net;
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

        // DELETE api/<DeleteMenuController>
        [HttpPost]
        public IActionResult Post([FromBody] string menuId)
        {
            string result = "Delete Success";
            string appSettingUrl = appSettings.Value.DeleteRichMenu;
            string deleteMenu = String.Format(appSettingUrl, menuId);
            var client = new RestClient(deleteMenu);
            var request = new RestRequest(Method.DELETE);
            try
            {
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                IRestResponse response = client.Execute(request);

                HttpStatusCode statusCode = response.StatusCode;
                if ((int)statusCode != 200)
                {
                    result = response.Content;
                    return NotFound(result);
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
