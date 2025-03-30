using System;
using System.Collections.Generic;
using System.Linq;
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
    public class OperationModeController : ControllerBase
    {
        private readonly OperationModeContext _context;

        public OperationModeController(OperationModeContext context)
        {
            _context = context;
        }

        // GET: api/OperationMode
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationMode>>> GetOperationModes([FromQuery]int? latest)
        {
            if (latest.HasValue)
            {
                return await _context.OperationModes
                    .OrderByDescending(e => e.Id)
                    .Take(latest.Value)
                    .ToListAsync();
            }
            return await _context.OperationModes.ToListAsync();
        }

        // GET: api/OperationMode/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperationMode>> GetOperationMode(long id)
        {
            var operationMode = await _context.OperationModes.FindAsync(id);

            if (operationMode == null)
            {
                return NotFound();
            }

            return operationMode;
        }
        
        // POST: api/OperationMode
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OperationMode>> PostOperationMode(OperationMode operationMode)
        {
            switch (operationMode.Mode)
            {
                // Allowed - continue processing.
                case "Automatic":
                case "Manual":
                    break;
                
                // Not allowed.
                default:
                    return BadRequest($"Unknown operation mode: {operationMode.Mode}");
            }
            _context.OperationModes.Add(operationMode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOperationModes", new { id = operationMode.Id }, operationMode);
        }
    }
}
