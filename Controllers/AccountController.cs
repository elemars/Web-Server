using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly ITokenService _tokenService;
        private readonly AlarmMonitoringService _alarmMonitoringService;

        public AccountController(UserContext context, ITokenService tokenService, AlarmMonitoringService alarmMonitoringService)
        {
            _context = context;
            _tokenService = tokenService;
            _alarmMonitoringService = alarmMonitoringService;
        }

        [Authorize(Roles = "admin,user")]
        [HttpGet("test")]
        public IActionResult Test()
        {
            Console.WriteLine("Test erfolgreich");
            return Ok(true);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginuser)
        {
            if (loginuser is null)
            {
                return BadRequest("Invalid client request");
            }
            else
            {
                Console.WriteLine(loginuser.Username);
                Console.WriteLine(loginuser.Password);
                // Benutzer in der Datenbank suchen
                var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginuser.Username);

                // Überprüfen, ob der Benutzer existiert
                if (userInDb == null)
                {
                    return Unauthorized(new { message = "Ungültiger Benutzername oder Passwort" });
                }

                // Überprüfen, ob das Passwort übereinstimmt
                if (!VerifyPassword(loginuser.Password, userInDb.Password))
                {
                    return Unauthorized(new { message = "Ungültiger Benutzername oder Passwort" });
                }
                // Rollen des Benutzers abrufen
                var roles = await GetRolesAsync(userInDb);

                if (roles != null)
                {
                    var token = _tokenService.GenerateLoginToken(userInDb.Username, roles);
                    // Token zurücksenden
                    return Ok(new { token });
                }
                else
                    return Unauthorized(new { message = "Keine Rolle zugewiesen" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Überprüfen, ob der Benutzername bereits existiert
            var userExists = await _context.Users.AnyAsync(u => u.Username == user.Username);
            if (userExists)
            {
                return BadRequest(new { message = "Benutzername ist bereits vergeben" });
            }

            // Passwort hashen
            user.Password = HashPassword(user.Password);

            // Benutzer zur Datenbank hinzufügen
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Erfolgsmeldung zurückgeben
            return Ok(new { message = "Registrierung erfolgreich" });
        }

        private string HashPassword(string password)
        {
            // Erzeugen eines zufälligen Salts
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Hashen des Passworts mit dem Salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Kombinieren von Salt und Hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Rückgabe des Base64-codierten Resultats
            return Convert.ToBase64String(hashBytes);
        }
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Konvertieren des gespeicherten Hashs aus Base64
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extrahieren des Salts
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hashen des eingegebenen Passworts mit dem extrahierten Salt
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Vergleich des resultierenden Hashs mit dem gespeicherten Hash
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;

            return true;
        }
        private async Task<string?> GetRolesAsync(User user)
        {
            if (_context != null)
            {
                var tempUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                var role = tempUser.Role;
                return role;
            }
            else return null;
        }

        [HttpGet("NFC/{UID}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<int>> GetUserByNFCTag(string UID)
        {
            Console.WriteLine($"UID: {UID}");
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(i => i.UID == UID);


            if (user == null)
            {
                return NotFound();
            }

            if (_alarmMonitoringService.GetAlarmStatus())
            {
                try
                {
                    _alarmMonitoringService.SetAlarmStatus(false);
                    var temp = new AlarmVerlauf
                    {
                        UserId = user.Id,
                        Timestamp = DateTime.Now,
                        Action = "deaktivieren"
                    };
                    await _context.AlarmVerlaufs.AddAsync(temp);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                try
                {
                    _alarmMonitoringService.SetAlarmStatus(true);
                    var temp = new AlarmVerlauf
                    {
                        UserId = user.Id,
                        Timestamp = DateTime.Now,
                        Action = "aktivieren"
                    };
                    await _context.AlarmVerlaufs.AddAsync(temp);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return user.Id;
        }
    }
}
