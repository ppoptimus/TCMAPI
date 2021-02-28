using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TCMAPI.Data;
using TCMAPI.Models;

namespace TCMAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class EventMessageController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Connected to TCM");
        }
        // POST api/EventMessage
        [HttpPost]
        public IActionResult Post([FromBody] RootEventMessage val)
        {
            var userId = val.events.Select(x => x.source.userId).FirstOrDefault().ToString();
            var messageType = val.events.Select(x => x.type).FirstOrDefault().ToString();
            switch (messageType)
            {
                case "message":
                    break;
                case "follow":
                    try
                    {
                        using (var db = new DataContext())
                        {
                            db.UserModels.Add(new UserModel { Id = 1, AppUserId = "0001", LineUserId = userId, UserStatus = "Followed", CreateDate = DateTime.Now.Date });
                            db.SaveChanges();
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError,ex.Message);
                    }
                    
                case "unfollow":
                    break;
                case "postback":
                    break;
                case "videoPlayComplete":
                    break;
                case "things":
                    break;
                default:
                    break;
            }
            return Ok();
        }

    }
}
