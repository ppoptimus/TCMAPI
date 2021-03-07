using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Net;
using TCMAPI.Models;

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkRichMenuToUserController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public LinkRichMenuToUserController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LinkMenuToUserModel val)
         {
            var result = "Success";
            if (ModelState.IsValid)
            {
                string userId = val.UserId;
                string richMenuId = val.RichMenuId;
                string resultFromLinkMenu = LinkMenu(userId, richMenuId);
                result = (resultFromLinkMenu == "Success") ? "Success" : resultFromLinkMenu;
            }
            
            return Ok(result);
        }

        protected string LinkMenu(string userId, string richMenuId)
        {
            string result = "Success";

            try
            {
                var client = new RestClient($"https://api.line.me/v2/bot/user/{userId}/richmenu/{richMenuId}");
                var request = new RestRequest(Method.POST);
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
