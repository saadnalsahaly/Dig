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

        // PUT: api/OperationMode/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOperationMode(long id, OperationMode operationMode)
        {
            if (id != operationMode.Id)
            {
                return BadRequest();
            }

            _context.Entry(operationMode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OperationModeExists(id))
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

        // DELETE: api/OperationMode/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperationMode(long id)
        {
            var operationMode = await _context.OperationModes.FindAsync(id);
            if (operationMode == null)
            {
                return NotFound();
            }

            _context.OperationModes.Remove(operationMode);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OperationModeExists(long id)
        {
            return _context.OperationModes.Any(e => e.Id == id);
        }
    }
}
