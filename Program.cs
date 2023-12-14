
using Server.Database;
using Server.Mqtt;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Server.Services;
using Server.Controllers;
using Server.Models;

namespace Server
{
    public class Program
    {
        public static string? Domain1 { get; private set; }
        public static string? Domain2 { get; private set; }
        public static string? SecretKey { get; private set; }


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();
            //______________________________________________________________________________________________________________________________________________________
            //MySection ist ein Bereich in der Datei appsettings.json, hier sind sensible Daten wie VAPID-Details und das Passwort für die MySQL-Datenbank hinterlegt
            //_______________________________________________________________________________________________________________________________________________________
            string? tempDomain;
            if (Environment.OSVersion.ToString().Contains("Pi"))
            {
                tempDomain = builder.Configuration["MySection:DomainPi"];
            }
            else
            {
                tempDomain = builder.Configuration["MySection:Domain1"];
            }
            Domain1 = tempDomain;
            tempDomain = builder.Configuration["MySection:Domain2"];
            Domain2 = tempDomain;
            var tempSecretkey = builder.Configuration["MySection:Key"];
            SecretKey = tempSecretkey;
            tempSecretkey = builder.Configuration["VAPID:publicKey"];
            AlarmMonitoringService.PublicKey = tempSecretkey;
            tempSecretkey = builder.Configuration["VAPID:privateKey"];
            AlarmMonitoringService.PrivateKey = tempSecretkey;
            tempSecretkey = builder.Configuration["VAPID:subject"];
            AlarmMonitoringService.Subject = tempSecretkey;
            //Console.WriteLine($"Domain: {Domain}");
            //Console.WriteLine($"SecretKey: {SecretKey}");
            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddControllers();
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Domain1,
                        ValidAudience = Domain2,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey))
                    };
                });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod().SetIsOriginAllowed((host) => true);
                    });
            });
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<UserContext>(opts => opts.UseMySql("Name=ConnectionStrings:UserConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql")));
            builder.Services.AddDbContext<FenstermonitoringContext>(options => options.UseMySql("Name=ConnectionStrings:DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql")));
            builder.Services.AddDbContext<PushSubscriptionContext>(options => options.UseMySql("Name=ConnectionStrings:UserConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql")));
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<INewState, NewState>();
            builder.Services.AddSingleton<AlarmMonitoringService>();

            var app = builder.Build();

            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {   //Swagger wird nur im Entwicklungsmodus verwendet
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            MqttClient.Initialize();

            app.Run();
        }

    }
}
