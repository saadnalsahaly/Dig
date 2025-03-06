using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dig;
using Dig.Models;

namespace Dig.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationContext _context;
        private readonly SseService<Notification> _sseService;

        public NotificationController(NotificationContext context, SseService<Notification> sseService)
        {
            _context = context;
            _sseService = sseService;
        }

        // GET: api/Notification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications([FromQuery]int? latest)
        {
            if (latest.HasValue)
            {
                return await _context.Notifications
                    .OrderByDescending(e => e.Id)
                    .Take(latest.Value)
                    .ToListAsync();
            }
            
            return await _context.Notifications.ToListAsync();
        }

        // GET: api/Notification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(long id)
        {
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            return notification;
        }
        
        [HttpGet("stream")]
        public async Task StreamUpdates()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            var responseStream = Response.Body;
            var cancellationToken = HttpContext.RequestAborted;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false  // You can set this to true if you want pretty print (indented JSON)
            };

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var update = await _sseService.WaitForUpdateAsync(cancellationToken);
                    var updateJson = JsonSerializer.Serialize(update, options);
                    var data = $"data: {updateJson}\n\n";
                    await Response.WriteAsync($"data: {updateJson}\n\n");
                    await responseStream.FlushAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Client disconnected, safely exit
            }
        }

        // PUT: api/Notification/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(long id, Notification notification)
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            _context.Entry(notification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Notification
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotifications", new { id = notification.Id }, notification);
        }

        // DELETE: api/Notification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(long id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotificationExists(long id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
