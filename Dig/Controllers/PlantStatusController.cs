using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dig;
using Dig.Data;
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
                    .OrderByDescending(e => e.DateTime)
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
        
        [HttpGet("Plant/{plantId}")]
        public async Task<ActionResult<IEnumerable<PlantStatus>>> GetByPlantId(string plantId)
        {
            var list = await _context.PlantStatuses
                .Where(ps => ps.PlantId == plantId)
                .OrderBy(ps => ps.DateTime)
                .ToListAsync();

            if (!list.Any())
                return NotFound($"No PlantStatus entries for plantId = '{plantId}'.");

            return Ok(list);
        }
        
        // GET: api/PlantStatus/stream
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
        
        // POST: api/PlantStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlantStatus>> PostPlantStatus(PlantStatus plantStatus)
        {
            _context.PlantStatuses.Add(plantStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantStatuses", new { id = plantStatus.Id }, plantStatus);
        }
    }
}
