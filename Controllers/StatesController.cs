using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models;
using Microsoft.AspNetCore.Authorization;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly FenstermonitoringContext _context;

        private readonly INewState _newState;
        private readonly AlarmMonitoringService _alarmMonitoringService;

        //private readonly NewState _newState;
        public FenstermonitoringContext Context {get; set;}

        public StatesController(FenstermonitoringContext context, INewState newState, AlarmMonitoringService alarmMonitoringService)
        {
            _context = context;
            _newState = newState;
            _alarmMonitoringService = alarmMonitoringService;
        }

        // GET: api/States
        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<IEnumerable<State>>> GetStates()
        {
          if (_context.States == null)
          {
              return NotFound();
          }
            return await _context.States.ToListAsync();
        }

        // GET: api/States/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<State>> GetState(int id)
        {
          if (_context.States == null)
          {
              return NotFound();
          }
            var state = await _context.States.FindAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            return state;
        }
        // GET: api/States/5
        [HttpGet("{Deviceid}")]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<State>> GetStateByDevice(int Deviceid)
        {
            if (_context.States == null)
            {
                return NotFound();
            }
            var state = await _context.States.OrderBy(i => i.Timestamp).LastOrDefaultAsync(i => i.Deviceid == Deviceid);

            if (state == null)
            {
                return NotFound();
            }

            return state;
        }

        // PUT: api/States/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutState(int id, State state)
        {
            if (id != state.Stateid)
            {
                return BadRequest();
            }

            _context.Entry(state).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(id))
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

        // POST: api/States
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<State>> PostState(State state)
        {
          if (_context.States == null)
          {
              return Problem("Entity set 'FenstermonitoringContext.States'  is null.");
          }
            var tempState = await _context.States.OrderBy(i => i.Timestamp).LastOrDefaultAsync(i => i.Deviceid == state.Deviceid);
            //Console.WriteLine($"new:  {state.Statevalue}");
            //Console.WriteLine($"temp: {tempState.Statevalue}");
            if (state.Statevalue == tempState.Statevalue)
            {
                return NoContent();
            }
            _context.States.Add(state);
            var device = await _context.Devices.FindAsync(state.Deviceid);
            if(device != null) device.Laststate = state.Statevalue;
            try
            {
                _newState.HandleEvent();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StateExists(state.Stateid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetState", new { id = state.Stateid }, state);
        }

        // DELETE: api/States/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteState(int id)
        {
            if (_context.States == null)
            {
                return NotFound();
            }
            var state = await _context.States.FindAsync(id);
            if (state == null)
            {
                return NotFound();
            }

            _context.States.Remove(state);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StateExists(int id)
        {
            return (_context.States?.Any(e => e.Stateid == id)).GetValueOrDefault();
        }
    }
}
