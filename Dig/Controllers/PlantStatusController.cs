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
    public class PlantStatusController : ControllerBase
    {
        private readonly PlantStatusContext _context;
        private readonly SseService<PlantStatus> _sseService;

        public PlantStatusController(PlantStatusContext context, SseService<PlantStatus> sseService)
        {
            _context = context;
            _sseService = sseService;
        }

        // GET: api/PlantStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantStatus>>> GetPlantStatuses([FromQuery]int? latest)
        {
            if (latest.HasValue)
            {
                return await _context.PlantStatuses
                    .OrderByDescending(e => e.Id)
                    .Take(latest.Value)
                    .ToListAsync();
            }
            
            return await _context.PlantStatuses.ToListAsync();
        }

        // GET: api/PlantStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlantStatus>> GetPlantStatus(long id)
        {
            var plantStatus = await _context.PlantStatuses.FindAsync(id);

            if (plantStatus == null)
            {
                return NotFound();
            }

            return plantStatus;
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

        // PUT: api/PlantStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlantStatus(long id, PlantStatus plantStatus)
        {
            if (id != plantStatus.Id)
            {
                return BadRequest();
            }

            _context.Entry(plantStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantStatusExists(id))
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

        // POST: api/PlantStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlantStatus>> PostPlantStatus(PlantStatus plantStatus)
        {
            _context.PlantStatuses.Add(plantStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantStatuses", new { id = plantStatus.Id }, plantStatus);
        }

        // DELETE: api/PlantStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlantStatus(long id)
        {
            var plantStatus = await _context.PlantStatuses.FindAsync(id);
            if (plantStatus == null)
            {
                return NotFound();
            }

            _context.PlantStatuses.Remove(plantStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlantStatusExists(long id)
        {
            return _context.PlantStatuses.Any(e => e.Id == id);
        }
    }
}
