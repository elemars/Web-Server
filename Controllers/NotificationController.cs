﻿using Server.Services;
using Server.Util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPush;
using Server.Database;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly PushSubscriptionContext _context;
        public NotificationController(PushSubscriptionContext pushSubscriptionContext)
        {
            _context = pushSubscriptionContext;
        }

        [Route("savePushSub")]
        [HttpPost]
        public async Task<IActionResult> SavePushSubscription([FromBody] PushSubscriptionToSave pushSubscription)
        {
            try
            {
                PushSubscriptionEntity pushSubscriptionEntity = new()
                    {
                    Auth = pushSubscription.auth,
                    P256dh = pushSubscription.p256dh,
                    Url = pushSubscription.url
                };

                await _context.PushSubscriptionEntities.AddAsync(pushSubscriptionEntity);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
