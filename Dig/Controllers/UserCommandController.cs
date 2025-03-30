using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UserCommandController : ControllerBase
    {
        private readonly UserCommandContext _userCommandContext;
        private readonly OperationModeContext _operationModeContext;

        public UserCommandController(UserCommandContext userCommandContext, OperationModeContext operationModeContext)
        {
            _userCommandContext = userCommandContext;
            _operationModeContext = operationModeContext;
        }

        // GET: api/UserCommands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCommand>>> GetUserCommands([FromQuery] int? latest)
        {
            if (latest.HasValue)
            {
                return await _userCommandContext.UserCommands
                    .OrderByDescending(c => c.Id)
                    .Take(latest.Value)
                    .ToListAsync();
            }

            return await _userCommandContext.UserCommands.ToListAsync();
        }

        // GET: api/UserCommands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserCommand>> GetUserCommand(long id)
        {
            var userCommand = await _userCommandContext.UserCommands.FindAsync(id);

            if (userCommand == null)
            {
                return NotFound();
            }

            return userCommand;
        }
        
        // POST: api/UserCommands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserCommand>> PostUserCommand(UserCommand userCommand)
        {
            // Retrieve the latest operation mode
            var latestMode = await _operationModeContext.OperationModes
                .OrderByDescending(m => m.Id) // Assuming the latest mode has the highest ID
                .FirstOrDefaultAsync();
    
            if (latestMode == null)
            {
                return BadRequest("Operation mode is not set.");
            }

            switch (latestMode.Mode)
            {
                case "Automatic":
                    return StatusCode(StatusCodes.Status403Forbidden, "User commands are not allowed in Automatic mode.");

                case "Manual":
                    // Allowed - continue processing
                    break;

                default:
                    return BadRequest($"Unknown operation mode: {latestMode.Mode}");
            }
            
            // If the mode is "Manual", proceed with saving the command
            _userCommandContext.UserCommands.Add(userCommand);
            await _userCommandContext.SaveChangesAsync();

            return CreatedAtAction("GetUserCommand", new { id = userCommand.Id }, userCommand);
        }
    }
}
