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
            var @type = "";
            double? latitude, longitude;

            switch (messageType)
            {
                case "message":
                    dynamic message = val.events.Select(x => x.message);
                    foreach (var item in message)
                    {
                        @type = item.@type.ToString();
                    }
                    if (@type == "location")
                    {
                        latitude = val.events.Select(x => x.message.latitude).First();
                        longitude = val.events.Select(x => x.message.longitude).First();
                        result = PushFlexLocation(userId);
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
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, ex.Message);
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
                        result = PushFlexMessageQ(userId);
                    }
                    else if (val.events.Select(x => x.postback.data == "C").First())
                    {
                        result = PushFlexMessageC(userId);
                    }
                    else if (val.events.Select(x => x.postback.data == "ContactUs").First())
                    {
                        result = PushFlexMessageContactUs(userId);
                    }
                    else if (val.events.Select(x => x.postback.data == "Benefits33").First())
                    {
                        result = PushFlexMessageBenefits(userId);
                    }
                    else if (val.events.Select(x => x.postback.data == "location").First())
                    {
                        result = SendActionLocation(userId);
                    }

                    break;
                default:
                    break;
            }
            return Ok(result);
        }

        protected string PushFlexMessageQ(string userId)
        {
            string result = "Ok";

            string CreateRichMenu = appSettings.Value.PushFlexMessageUrl;
            var client = new RestClient(CreateRichMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n    \"to\": \"" + userId + "\",\r\n    \"messages\": [\r\n        {\r\n            \"type\": \"flex\",\r\n            \"altText\": \"this is a flex message\",\r\n            \"contents\": {\r\n  \"type\": \"carousel\",\r\n  \"contents\": [\r\n    {\r\n      \"type\": \"bubble\",\r\n      \"size\": \"kilo\",\r\n      \"hero\": {\r\n        \"type\": \"image\",\r\n        \"url\": \"https://www.img.in.th/images/9f2fd4cee8bba418d9c8bc115fe89e6a.jpg\",\r\n        \"flex\": 1,\r\n        \"size\": \"full\",\r\n        \"aspectRatio\": \"20:13\",\r\n        \"aspectMode\": \"cover\",\r\n        \"backgroundColor\": \"#FFFFFFFF\",\r\n        \"action\": {\r\n          \"type\": \"uri\",\r\n          \"label\": \"Line\",\r\n          \"uri\": \"https://linecorp.com/\"\r\n        }\r\n      },\r\n      \"body\": {\r\n        \"type\": \"box\",\r\n        \"layout\": \"vertical\",\r\n        \"contents\": [\r\n          {\r\n            \"type\": \"text\",\r\n            \"text\": \"คำถามพบบ่อย\",\r\n            \"weight\": \"bold\",\r\n            \"size\": \"xl\",\r\n            \"color\": \"#000000\",\r\n            \"contents\": []\r\n          }\r\n        ]\r\n      },\r\n      \"footer\": {\r\n        \"type\": \"box\",\r\n        \"layout\": \"vertical\",\r\n        \"flex\": 0,\r\n        \"spacing\": \"sm\",\r\n        \"contents\": [\r\n          {\r\n            \"type\": \"button\",\r\n            \"action\": {\r\n              \"type\": \"uri\",\r\n              \"label\": \"ขั้นตอนสมัคร ม.33\",\r\n              \"uri\": \"https://linecorp.com/\"\r\n            },\r\n            \"height\": \"sm\",\r\n            \"style\": \"primary\"\r\n          },\r\n          {\r\n            \"type\": \"button\",\r\n            \"action\": {\r\n              \"type\": \"uri\",\r\n              \"label\": \"ขั้นตอนสมัคร ม.39\",\r\n              \"uri\": \"https://linecorp.com\"\r\n            },\r\n            \"height\": \"sm\",\r\n            \"style\": \"primary\"\r\n          },\r\n          {\r\n            \"type\": \"button\",\r\n            \"action\": {\r\n              \"type\": \"uri\",\r\n              \"label\": \"ขั้นตอนสมัคร ม.40\",\r\n              \"uri\": \"https://linecorp.com\"\r\n            },\r\n            \"height\": \"sm\",\r\n            \"style\": \"primary\"\r\n          },\r\n          {\r\n            \"type\": \"spacer\"\r\n          }\r\n        ]\r\n      }\r\n    },\r\n    {\r\n      \"type\": \"bubble\",\r\n      \"size\": \"kilo\",\r\n      \"hero\": {\r\n        \"type\": \"image\",\r\n        \"url\": \"https://www.img.in.th/images/b854e386ee61d1a2ffeb6b137202e1fa.jpg\",\r\n        \"flex\": 1,\r\n        \"size\": \"full\",\r\n        \"aspectRatio\": \"20:13\",\r\n        \"aspectMode\": \"cover\",\r\n        \"backgroundColor\": \"#FFFFFFFF\",\r\n        \"action\": {\r\n          \"type\": \"uri\",\r\n          \"label\": \"Line\",\r\n          \"uri\": \"tel:1506\"\r\n        }\r\n      },\r\n      \"body\": {\r\n        \"type\": \"box\",\r\n        \"layout\": \"vertical\",\r\n        \"contents\": [\r\n          {\r\n            \"type\": \"text\",\r\n            \"text\": \"ติดต่อประกันสังคม\",\r\n            \"weight\": \"bold\",\r\n            \"size\": \"xl\",\r\n            \"color\": \"#000000\",\r\n            \"contents\": []\r\n          }\r\n        ]\r\n      },\r\n      \"footer\": {\r\n        \"type\": \"box\",\r\n        \"layout\": \"vertical\",\r\n        \"flex\": 0,\r\n        \"spacing\": \"sm\",\r\n        \"contents\": [\r\n          {\r\n            \"type\": \"button\",\r\n            \"action\": {\r\n              \"type\": \"uri\",\r\n              \"label\": \"สายด่วน 1506\",\r\n              \"uri\": \"tel:1506\"\r\n            },\r\n            \"height\": \"sm\",\r\n            \"style\": \"primary\"\r\n          },\r\n          {\r\n            \"type\": \"button\",\r\n            \"action\": {\r\n              \"type\": \"uri\",\r\n              \"label\": \"ส่งอีเมลติดต่อ\",\r\n              \"uri\": \"https://linecorp.com\"\r\n            },\r\n            \"height\": \"sm\",\r\n            \"style\": \"primary\"\r\n          },\r\n          {\r\n            \"type\": \"button\",\r\n            \"action\": {\r\n              \"type\": \"postback\",\r\n              \"label\": \"ค้นหาที่ทำการ\",\r\n              \"data\": \"location\"\r\n            },\r\n            \"height\": \"sm\",\r\n            \"style\": \"primary\"\r\n          },\r\n          {\r\n            \"type\": \"spacer\"\r\n          }\r\n        ]\r\n      }\r\n    }\r\n  ]\r\n}\r\n        }\r\n    ]\r\n}", ParameterType.RequestBody);
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

        protected string PushFlexMessageC(string userId)
        {
            string result = "Ok";

            string CreateRichMenu = appSettings.Value.PushFlexMessageUrl;
            var client = new RestClient(CreateRichMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n    \"to\": \"" + userId + "\",\r\n    \"messages\": [\r\n        {\r\n            \"type\": \"flex\",\r\n            \"altText\": \"this is a flex message\",\r\n            \"contents\": {\r\n                \"type\": \"carousel\",\r\n                \"contents\": [\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/166b6de70db531d6464472feb4bd84b4.jpg\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"ผู้ประกันตนมาตรา 33\",\r\n                                    \"weight\": \"bold\",\r\n                                    \"size\": \"xl\",\r\n                                    \"contents\": []\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"สิทธิรักษาพยาบาล\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"กองทุนชราภาพ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"ทุพพลภาพหรือเสียชีวิต\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    },\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/7ca6f44ebcc0d4e0539aef314467f133.jpg\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"ผู้ประกันตนมาตรา 39\",\r\n                                    \"weight\": \"bold\",\r\n                                    \"size\": \"xl\",\r\n                                    \"contents\": []\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"สิทธิรักษาพยาบาล\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"กองทุนชราภาพ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"ทุพพลภาพหรือเสียชีวิต\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    },\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/8871af6a29da848bcd9c7af16e6b390f.jpg\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"ผู้ประกันตนมาตรา 40\",\r\n                                    \"weight\": \"bold\",\r\n                                    \"size\": \"xl\",\r\n                                    \"contents\": []\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"สิทธิรักษาพยาบาล\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"กองทุนชราภาพ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"ทุพพลภาพหรือเสียชีวิต\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    }\r\n                ]\r\n            }\r\n        }\r\n    ]\r\n}", ParameterType.RequestBody);
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

        protected string PushFlexMessageContactUs(string userId)
        {
            string result = "OK";

            string CreateRichMenu = appSettings.Value.PushFlexMessageUrl;
            var client = new RestClient(CreateRichMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n    \"to\": \"" + userId + "\",\r\n    \"messages\": [\r\n        {\r\n            \"type\": \"flex\",\r\n            \"altText\": \"this is a flex message\",\r\n            \"contents\": {\r\n  \"type\": \"bubble\",\r\n  \"size\": \"kilo\",\r\n  \"hero\": {\r\n    \"type\": \"image\",\r\n    \"url\": \"https://www.img.in.th/images/b854e386ee61d1a2ffeb6b137202e1fa.jpg\",\r\n    \"flex\": 1,\r\n    \"size\": \"full\",\r\n    \"aspectRatio\": \"20:13\",\r\n    \"aspectMode\": \"cover\",\r\n    \"backgroundColor\": \"#FFFFFFFF\",\r\n    \"action\": {\r\n      \"type\": \"uri\",\r\n      \"label\": \"Line\",\r\n      \"uri\": \"tel:1506\"\r\n    }\r\n  },\r\n  \"body\": {\r\n    \"type\": \"box\",\r\n    \"layout\": \"vertical\",\r\n    \"contents\": [\r\n      {\r\n        \"type\": \"text\",\r\n        \"text\": \"ติดต่อประกันสังคม\",\r\n        \"weight\": \"bold\",\r\n        \"size\": \"xl\",\r\n        \"color\": \"#000000\",\r\n        \"contents\": []\r\n      }\r\n    ]\r\n  },\r\n  \"footer\": {\r\n    \"type\": \"box\",\r\n    \"layout\": \"vertical\",\r\n    \"flex\": 0,\r\n    \"spacing\": \"sm\",\r\n    \"contents\": [\r\n      {\r\n        \"type\": \"button\",\r\n        \"action\": {\r\n          \"type\": \"uri\",\r\n          \"label\": \"สายด่วน 1506\",\r\n          \"uri\": \"tel:1506\"\r\n        },\r\n        \"height\": \"sm\",\r\n        \"style\": \"primary\"\r\n      },\r\n      {\r\n        \"type\": \"button\",\r\n        \"action\": {\r\n          \"type\": \"uri\",\r\n          \"label\": \"ส่งอีเมลติดต่อ\",\r\n          \"uri\": \"https://linecorp.com\"\r\n        },\r\n        \"height\": \"sm\",\r\n        \"style\": \"primary\"\r\n      },\r\n      {\r\n        \"type\": \"button\",\r\n        \"action\": {\r\n          \"type\": \"postback\",\r\n          \"label\": \"ค้นหาที่ทำการ\",\r\n          \"data\": \"location\"\r\n        },\r\n        \"height\": \"sm\",\r\n        \"style\": \"primary\"\r\n      },\r\n      {\r\n        \"type\": \"spacer\"\r\n      }\r\n    ]\r\n  }\r\n}\r\n        }\r\n    ]\r\n}", ParameterType.RequestBody);
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

        protected string PushFlexMessageBenefits(string userId)
        {
            string result = "OK";
            string CreateRichMenu = appSettings.Value.PushFlexMessageUrl;
            var client = new RestClient(CreateRichMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n    \"to\": \"" + userId + "\",\r\n    \"messages\": [\r\n        {\r\n            \"type\": \"flex\",\r\n            \"altText\": \"this is a flex message\",\r\n            \"contents\": {\r\n                \"type\": \"carousel\",\r\n                \"contents\": [\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"direction\": \"ltr\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/1cf2979b78167333fe2b99083184ed24.png\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"เจ็บป่วยอุบัติเหตุ\",\r\n                                    \"size\": \"md\",\r\n                                    \"contents\": []\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"รายละเอียดสิทธิ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"ขั้นตอนการเบิกจ่าย\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"เงื่อนไขอื่นๆ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"spacer\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    },\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"direction\": \"ltr\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/949da47278990eebeae6563753cf13e6.png\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"ทันตกรรม\",\r\n                                    \"size\": \"md\",\r\n                                    \"contents\": []\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"รายละเอียดสิทธิ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"ขั้นตอนการเบิกจ่าย\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"เงื่อนไขอื่นๆ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"spacer\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    },\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"size\": \"kilo\",\r\n                        \"direction\": \"ltr\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://www.img.in.th/images/dd640d9756f9cfd48c77985d1b59ed59.png\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"label\": \"Line\",\r\n                                \"uri\": \"https://linecorp.com/\"\r\n                            }\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"คลอดและสงเคราะห์บุตร\",\r\n                                    \"size\": \"md\",\r\n                                    \"contents\": []\r\n                                }\r\n                            ]\r\n                        },\r\n                        \"footer\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"flex\": 0,\r\n                            \"spacing\": \"sm\",\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"รายละเอียดสิทธิ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"ขั้นตอนการเบิกจ่าย\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"button\",\r\n                                    \"action\": {\r\n                                        \"type\": \"uri\",\r\n                                        \"label\": \"เงื่อนไขอื่นๆ\",\r\n                                        \"uri\": \"https://linecorp.com\"\r\n                                    },\r\n                                    \"color\": \"#3E7FDFFF\",\r\n                                    \"height\": \"sm\",\r\n                                    \"style\": \"primary\"\r\n                                },\r\n                                {\r\n                                    \"type\": \"spacer\"\r\n                                }\r\n                            ]\r\n                        }\r\n                    }\r\n                ]\r\n            }\r\n        }\r\n    ]\r\n}", ParameterType.RequestBody);
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

        protected string SendActionLocation(string userId)
        {
            string result = "OK";
            string CreateRichMenu = appSettings.Value.PushFlexMessageUrl;
            var client = new RestClient(CreateRichMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n \"to\": \"" + userId + "\",\r\n \"messages\": [\r\n  {\r\n   \"type\": \"text\",\r\n   \"text\": \"คลิกปุ่ม \\\"Your location\\\" ด้านล่างเพื่อค้นหาสถานที่ใกล้เคียง\",\r\n   \"quickReply\": {\r\n    \"items\": [\r\n     {\r\n      \"type\": \"action\",\r\n      \"action\": {\r\n       \"type\":\"location\",\r\n       \"label\":\"Your location\"\r\n      }\r\n     }\r\n    ]\r\n   }\r\n  }\r\n ]\r\n}", ParameterType.RequestBody);
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

        protected string PushFlexLocation(string userId)
        {
            string result = "OK";

            string CreateRichMenu = appSettings.Value.PushFlexMessageUrl;
            var client = new RestClient(CreateRichMenu);
            var request = new RestRequest(Method.POST);
            try
            {
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", appSettings.Value.LineChannelAccessToken);
                request.AddParameter("application/json", "{\r\n    \"to\": \"" + userId + "\",\r\n    \"messages\": [\r\n        {\r\n            \"type\": \"flex\",\r\n            \"altText\": \"this is a flex message\",\r\n            \"contents\": {\r\n                \"type\": \"carousel\",\r\n                \"contents\": [\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://lh5.googleusercontent.com/p/AF1QipO-N3DgdZK2KJ_v6EtpQfLiIsLj8miNkPykOJrP=w195-h120-p-k-no\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\"\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"uri\": \"https://www.google.co.th/maps/place/Social+Security+Office/@13.8349791,100.5155056,13z/data=!4m8!1m2!2m1!1z4Liq4Liy4LiC4Liy4Lib4Lij4Liw4LiB4Lix4LiZ4Liq4Lix4LiH4LiE4Lih!3m4!1s0x30e29b61170f0de5:0x2a0d152da72dc9d4!8m2!3d13.8455254!4d100.525599?hl=en&authuser=0\"\r\n                            },\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"สำนักงานประกันสังคม\",\r\n                                    \"weight\": \"bold\",\r\n                                    \"size\": \"xl\",\r\n                                    \"contents\": []\r\n                                },\r\n                                {\r\n                                    \"type\": \"box\",\r\n                                    \"layout\": \"vertical\",\r\n                                    \"spacing\": \"sm\",\r\n                                    \"margin\": \"lg\",\r\n                                    \"contents\": [\r\n                                        {\r\n                                            \"type\": \"box\",\r\n                                            \"layout\": \"baseline\",\r\n                                            \"spacing\": \"sm\",\r\n                                            \"contents\": [\r\n                                                {\r\n                                                    \"type\": \"text\",\r\n                                                    \"text\": \"ที่อยู่\",\r\n                                                    \"size\": \"sm\",\r\n                                                    \"color\": \"#AAAAAA\",\r\n                                                    \"flex\": 1,\r\n                                                    \"contents\": []\r\n                                                },\r\n                                                {\r\n                                                    \"type\": \"text\",\r\n                                                    \"text\": \"ตำบล ตลาดขวัญ อำเภอเมืองนนทบุรี นนทบุรี 11000\",\r\n                                                    \"size\": \"sm\",\r\n                                                    \"color\": \"#666666\",\r\n                                                    \"flex\": 5,\r\n                                                    \"wrap\": true,\r\n                                                    \"contents\": []\r\n                                                }\r\n                                            ]\r\n                                        }\r\n                                    ]\r\n                                }\r\n                            ]\r\n                        }\r\n                    },\r\n                    {\r\n                        \"type\": \"bubble\",\r\n                        \"hero\": {\r\n                            \"type\": \"image\",\r\n                            \"url\": \"https://lh5.googleusercontent.com/p/AF1QipODN1RYKr25pPRCxFx5CRvh9hpnxbPaakyShO9x=w195-h120-p-k-no\",\r\n                            \"size\": \"full\",\r\n                            \"aspectRatio\": \"20:13\",\r\n                            \"aspectMode\": \"cover\"\r\n                        },\r\n                        \"body\": {\r\n                            \"type\": \"box\",\r\n                            \"layout\": \"vertical\",\r\n                            \"action\": {\r\n                                \"type\": \"uri\",\r\n                                \"uri\": \"https://www.google.co.th/maps/place/Social+Security+Bangkok+Office+Area+2/@13.8349791,100.5155056,13z/data=!4m8!1m2!2m1!1z4Liq4Liy4LiC4Liy4Lib4Lij4Liw4LiB4Lix4LiZ4Liq4Lix4LiH4LiE4Lih!3m4!1s0x30e29c964eb21e01:0xd76ff362a3fad4cd!8m2!3d13.8466847!4d100.5435452?hl=en&authuser=0\"\r\n                            },\r\n                            \"contents\": [\r\n                                {\r\n                                    \"type\": \"text\",\r\n                                    \"text\": \"สำนักงานประกันสังคมกรุงเทพมหานครพื้นที่ 2\",\r\n                                    \"weight\": \"bold\",\r\n                                    \"size\": \"xl\",\r\n                                    \"contents\": []\r\n                                },\r\n                                {\r\n                                    \"type\": \"box\",\r\n                                    \"layout\": \"vertical\",\r\n                                    \"spacing\": \"sm\",\r\n                                    \"margin\": \"lg\",\r\n                                    \"contents\": [\r\n                                        {\r\n                                            \"type\": \"box\",\r\n                                            \"layout\": \"baseline\",\r\n                                            \"spacing\": \"sm\",\r\n                                            \"contents\": [\r\n                                                {\r\n                                                    \"type\": \"text\",\r\n                                                    \"text\": \"ที่อยู่\",\r\n                                                    \"size\": \"sm\",\r\n                                                    \"color\": \"#AAAAAA\",\r\n                                                    \"flex\": 1,\r\n                                                    \"contents\": []\r\n                                                },\r\n                                                {\r\n                                                    \"type\": \"text\",\r\n                                                    \"text\": \"แขวง ลาดยาว เขตจตุจักร กรุงเทพมหานคร 10900\",\r\n                                                    \"size\": \"sm\",\r\n                                                    \"color\": \"#666666\",\r\n                                                    \"flex\": 5,\r\n                                                    \"wrap\": true,\r\n                                                    \"contents\": []\r\n                                                }\r\n                                            ]\r\n                                        }\r\n                                    ]\r\n                                }\r\n                            ]\r\n                        }\r\n                    }\r\n                ]\r\n            }\r\n        }\r\n    ]\r\n}", ParameterType.RequestBody);
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
