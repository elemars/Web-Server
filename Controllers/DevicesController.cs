using Server.Database;
using Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly FenstermonitoringContext _context;

        public DevicesController(FenstermonitoringContext context)
        {
            _context = context;
        }

        // GET: api/Devices
        [HttpGet]
        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            return await _context.Devices.ToListAsync();
        }

        // GET: api/Devices/5
        [HttpGet("id/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Device>> GetDeviceById(int id)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // GET: api/Devices/5
        [HttpGet("mac/{Macadress}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<int>> GetDeviceByMac(string MACa)
        {
            Console.WriteLine($"MACa: {MACa}");
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var device = await _context.Devices.FirstOrDefaultAsync(i => i.Macadress == MACa);
            //Console.WriteLine(device.Macadress);
            if (device == null)
            {
                return NotFound();
            }

            return device.Deviceid;
        }

        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("new/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutDeviceNew(int id, Device device)
        {
            Console.WriteLine("falsche Methode");
            if (id != device.Deviceid)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
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

        // PUT: api/Devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("voltage/{id}/{BatteryLevel}/{LastState}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutDeviceVoltage(int id, string BatteryLevel, int Laststate)
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ",";
            decimal voltage = decimal.Parse(BatteryLevel);
            //Console.WriteLine($"ID:               {id}");
            //Console.WriteLine($"String: {BatteryLevel}");
            //Console.WriteLine($"voltage: {voltage}");
            var device = await _context.Devices.FindAsync(id);

            //Debug.WriteLine("Batteriespannung: ", BatteryLevel);

            if (device == null)
            {
                return BadRequest();
            }

            device.Batterylevel = voltage;
            device.Laststate = Laststate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
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

        // POST: api/Devices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
            if (_context.Devices == null)
            {
                return Problem("Entity set 'FenstermonitoringContext.Devices'  is null.");
            }
            _context.Devices.Add(device);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeviceExists(device.Deviceid))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //Console.WriteLine(device.Deviceid);
            //Console.WriteLine($"{nameof(GetDeviceById)}/id");
            //return CreatedAtAction("GetDeviceById/id/?{device.Deviceid}", null);
            return CreatedAtAction("GetDeviceById", new { id = device.Deviceid }, device);
            //return device;
        }

        // DELETE: api/Devices/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceExists(int id)
        {
            return (_context.Devices?.Any(e => e.Deviceid == id)).GetValueOrDefault();
        }
    }
}
