using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Dig;
using Dig.Data;
using Environment = Dig.Models.Environment;

namespace Dig.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly EnvironmentContext _context;
        private readonly SseService<Environment> _sseService;

        public EnvironmentController(EnvironmentContext context, SseService<Environment> sseService)
        {
            _context = context;
            _sseService = sseService;
        }

        // GET: api/Environment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Environment>>> GetEnvironments([FromQuery] int? latest)
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
        public async Task<ActionResult<Environment>> GetEnvironment(long id)
        {
            var environmentData = await _context.Environments.FindAsync(id);

            if (environmentData == null)
            {
                return NotFound();
            }

            return environmentData;
        }
        
        // GET: api/Environment/stream
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
        
        // POST: api/Environment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Environment>> PostEnvironment(Environment environment)
        {
            _context.Environments.Add(environment);
            await _context.SaveChangesAsync();

            _sseService.AddUpdate(environment);

            return CreatedAtAction(nameof(GetEnvironments), new { id = environment.Id }, environment);
        }
    }
}
