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
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly EnvironmentContext _context;

        public EnvironmentController(EnvironmentContext context)
        {
            _context = context;
        }

        // GET: api/Environment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnvironmentData>>> GetEnvironments()
        {
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

            return CreatedAtAction("GetEnvironmentData", new { id = environmentData.Id }, environmentData);
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
