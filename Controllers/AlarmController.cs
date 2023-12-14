using Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlarmController : ControllerBase
    {
        AlarmMonitoringService _alarmMonitoringService;
        public AlarmController(AlarmMonitoringService AlarmMonitoringService) 
        {
            _alarmMonitoringService = AlarmMonitoringService;
        }
        // GET: api/<AlarmController>
        [HttpGet]
        //[Authorize(Roles = "admin, user")]
        public bool Get()
        {
            Console.WriteLine($"Alarm-Status Server: {_alarmMonitoringService.GetAlarmStatus()}");
            return _alarmMonitoringService.GetAlarmStatus();
        }
        // POST api/<AlarmController>
        [HttpPost]
        [Authorize(Roles = "admin, user")]
        public void Post([FromBody] bool active)
        {
            Console.WriteLine($"Alarm-Status vorher: {_alarmMonitoringService.GetAlarmStatus()}");
            _alarmMonitoringService.SetAlarmStatus(active);
            Console.WriteLine($"Alarm-Status nachher: {_alarmMonitoringService.GetAlarmStatus()}");
        }
    }
}
