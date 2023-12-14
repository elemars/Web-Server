using FenstermonitoringAPI.Database;
using FenstermonitoringAPI.Mqtt;
using FenstermonitoringAPI.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FenstermonitoringAPI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllers();

            // Authentifizierung hinzufügen
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Security.Domain,
                        ValidAudience = Security.Domain,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Security.SecretKey))
                    };
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<FenstermonitoringContext>(options => options.UseMySql("Name=ConnectionStrings:DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql")));
        }

        public void Configure(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            MqttClient.Initialize();
        }
    }
}
