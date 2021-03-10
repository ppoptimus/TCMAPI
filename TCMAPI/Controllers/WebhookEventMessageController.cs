using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TCMAPI.Data;
using TCMAPI.Models;

namespace TCMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookEventMessageController : ControllerBase
    {
        private readonly IOptions<AppSettingModel> appSettings;
        public WebhookEventMessageController(IOptions<AppSettingModel> app)
        {
            appSettings = app;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Connected to TCM");
        }
        // POST api/EventMessage
        [HttpPost]
        public IActionResult Post([FromBody] RootEventMessageModel val)
        {
            var result = "no action";
            var userId = val.events.Select(x => x.source.userId).FirstOrDefault().ToString();
            var messageType = val.events.Select(x => x.type).FirstOrDefault().ToString();

            switch (messageType)
            {
                case "message":
                    if(val.events.Select(x=>x.message.text == "Q").First())
                    {
                        result = PushFlexMessageToUser(userId);
                    }
                    
                    break;
                case "follow":
                    try
                    {
                        using (var db = new DataContext())
                        {
                            db.LineUsers.Add(new LineUser { LineUserId = userId, UserStatus = "Followed", CreateDate = DateTime.Now.Date });
                            db.SaveChanges();
                            result = userId;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError,ex.Message);
                    }
                    
                case "unfollow":
                    try
                    {
                        using (var db = new DataContext())
                        {
                            db.LineUsers.Remove(db.LineUsers.Where(x => x.LineUserId == userId).FirstOrDefault());
                            db.SaveChanges();
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, ex.Message);
                    }
                    
                case "postback":
                    if (val.events.Select(x => x.postback.data == "Q").First())
                    {
                        result = PushFlexMessageToUser(userId);
                    }
                    break;
                case "things":
                    break;
                default:
                    break;
            }
            return Ok(result);
        }

        protected string PushFlexMessageToUser(string userId)
        {
            string result = "Ok";

            string CreateRichMenu = appSettings.Value.PushFlexMessageUrl;
            var client = new RestClient(CreateRichMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n    \"to\": \"U37d3a5f0191f7ccc298979b769971f24\",\r\n    \"messages\": [\r\n        {\r\n            \"type\": \"flex\",\r\n            \"altText\": \"this is a flex message\",\r\n            \"contents\": {\r\n                \"type\": \"carousel\",\r\n                \"contents\": [\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/b854e386ee61d1a2ffeb6b137202e1fa.jpg\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"นี่คือ Flex Message\",\r\n                                    \"weight\": \"bold\",\r\n                                    \"size\": \"xl\",\r\n                                    \"contents\": []\r\n                                },\r\n                                {\r\n                                    \"type\": \"separator\",\r\n                                    \"margin\": \"sm\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"Contact us\",\r\n                                    \"size\": \"sm\",\r\n                                    \"contents\": []\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"CALL\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"md\",\r\n                                    \"style\": \"primary\",\r\n                                    \"gravity\": \"bottom\",\r\n                                    \"position\": \"relative\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"spacer\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    },\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/9f2fd4cee8bba418d9c8bc115fe89e6a.jpg\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"นี่คือ Flex Message\",\r\n                                    \"weight\": \"bold\",\r\n                                    \"size\": \"xl\",\r\n                                    \"contents\": [\r\n                                        {\r\n                                            \"type\": \"span\",\r\n                                            \"text\": \"นี่คือ Flex Message\"\r\n                                        }\r\n                                    ]\r\n                                },\r\n                                {\r\n                                    \"type\": \"separator\",\r\n                                    \"margin\": \"sm\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"นี่คือ Flex Message\",\r\n                                    \"contents\": [\r\n                                        {\r\n                                            \"type\": \"span\",\r\n                                            \"text\": \"Q&A\",\r\n                                            \"size\": \"sm\",\r\n                                            \"style\": \"normal\"\r\n                                        }\r\n                                    ]\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"More\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"md\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"spacer\",\r\n                                    \"size\": \"sm\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    }\r\n                ]\r\n            }\r\n        }\r\n    ]\r\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                result = response.Content;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

    }
}
