using Microsoft.AspNetCore.Mvc;
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
                    break;
                case "follow":
                    try
                    {
                        using (var db = new DataContext())
                        {
                            db.LineUsers.Add(new LineUserModel { LineUserId = userId, UserStatus = "Followed", CreateDate = DateTime.Now.Date });
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
                    break;
                case "things":
                    break;
                default:
                    break;
            }
            return Ok(result);
        }

    }
}
