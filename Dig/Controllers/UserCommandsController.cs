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
    public class UserCommandsController : ControllerBase
    {
        private readonly UserCommandContext _context;

        public UserCommandsController(UserCommandContext context)
        {
            _context = context;
        }

        // GET: api/UserCommands
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCommand>>> GetUserCommands([FromQuery] int? latest)
        {
            if (latest.HasValue)
            {
                return await _context.UserCommands
                    .OrderByDescending(c => c.Id)
                    .Take(latest.Value)
                    .ToListAsync();
            }

            return await _context.UserCommands.ToListAsync();
        }


        // GET: api/UserCommands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserCommand>> GetUserCommand(long id)
        {
            var userCommand = await _context.UserCommands.FindAsync(id);

            if (userCommand == null)
            {
                return NotFound();
            }

            return userCommand;
        }

        // PUT: api/UserCommands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserCommand(long id, UserCommand userCommand)
        {
            if (id != userCommand.Id)
            {
                return BadRequest();
            }

            _context.Entry(userCommand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserCommandExists(id))
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

        // POST: api/UserCommands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserCommand>> PostUserCommand(UserCommand userCommand)
        {
            _context.UserCommands.Add(userCommand);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserCommand", new { id = userCommand.Id }, userCommand);
        }

        // DELETE: api/UserCommands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserCommand(long id)
        {
            var userCommand = await _context.UserCommands.FindAsync(id);
            if (userCommand == null)
            {
                return NotFound();
            }

            _context.UserCommands.Remove(userCommand);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserCommandExists(long id)
        {
            return _context.UserCommands.Any(e => e.Id == id);
        }
    }
}
