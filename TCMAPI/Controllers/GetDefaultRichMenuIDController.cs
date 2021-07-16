using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net;
using TCMAPI.Models;


namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDefaultRichMenuIDController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public GetDefaultRichMenuIDController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string str = "";
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(appSettings.Value.GetDefaultRichMenuIDUrl);
                string lineAccessToken = appSettings.Value.LineChannelAccessToken;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", lineAccessToken);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    str = streamReader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                str = ex.Message;
                return NotFound(str);
            }


            return Ok(str);
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

    }
}
