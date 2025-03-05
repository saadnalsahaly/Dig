using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Dig;
using Dig.Models;

namespace Dig.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly EnvironmentContext _context;
        private SseService<EnvironmentData> _sseService;

        public EnvironmentController(EnvironmentContext context, SseService<EnvironmentData> sseService)
        {
            _context = context;
            _sseService = sseService;
        }

        // GET: api/Environment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnvironmentData>>> GetEnvironments([FromQuery] int? latest)
        {
            if (latest.HasValue)
            {
                return await _context.Environments
                    .OrderByDescending(e => e.Id)
                    .Take(latest.Value)
                    .ToListAsync();
            }
            
            return await _context.Environments.ToListAsync();
        }

        // GET: api/Environment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EnvironmentData>> GetEnvironmentData(long id)
        {
            var environmentData = await _context.Environments.FindAsync(id);

            if (environmentData == null)
            {
                return NotFound();
            }

            return environmentData;
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
                int i = 0;
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    var update = await _sseService.WaitForUpdateAsync(cancellationToken);
                    var updateJson = JsonSerializer.Serialize(update, options);
                    var data = $"data: {updateJson}\n\n";
                    await Response.WriteAsync($"data: {updateJson}\n\n");
                    await responseStream.FlushAsync();
                    i++;
                }
            }
            catch (OperationCanceledException)
            {
                // Client disconnected, safely exit
            }
        }


        // PUT: api/Environment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnvironmentData(long id, EnvironmentData environmentData)
        {
            if (id != environmentData.Id)
            {
                return BadRequest();
            }

            _context.Entry(environmentData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnvironmentDataExists(id))
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

        // POST: api/Environment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EnvironmentData>> PostEnvironmentData(EnvironmentData environmentData)
        {
            _context.Environments.Add(environmentData);
            await _context.SaveChangesAsync();

            _sseService.AddUpdate(environmentData);

            return CreatedAtAction(nameof(GetEnvironments), new { id = environmentData.Id }, environmentData);
        }

        // DELETE: api/Environment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvironmentData(long id)
        {
            var environmentData = await _context.Environments.FindAsync(id);
            
            if (environmentData == null)
            {
                return NotFound();
            }

            _context.Environments.Remove(environmentData);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool EnvironmentDataExists(long id)
        {
            return _context.Environments.Any(e => e.Id == id);
        }
    }
}
