using Inventario.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();

            try
            {
                // Configuración de la base de datos MySQL usando variables de entorno de Railway
                var mysqlHost = Environment.GetEnvironmentVariable("MYSQLHOST") ?? throw new InvalidOperationException("MYSQLHOST no configurado");
                var mysqlPort = Environment.GetEnvironmentVariable("MYSQLPORT") ?? throw new InvalidOperationException("MYSQLPORT no configurado");
                var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? throw new InvalidOperationException("MYSQL_DATABASE no configurado");
                var mysqlUser = Environment.GetEnvironmentVariable("MYSQLUSER") ?? throw new InvalidOperationException("MYSQLUSER no configurado");
                var mysqlPassword = Environment.GetEnvironmentVariable("MYSQLPASSWORD") ?? throw new InvalidOperationException("MYSQLPASSWORD no configurado");

                var connectionString = $"Server={mysqlHost};" +
                                     $"Port={mysqlPort};" +
                                     $"Database={mysqlDatabase};" +
                                     $"User={mysqlUser};" +
                                     $"Password={mysqlPassword};" +
                                     "AllowPublicKeyRetrieval=true;" +
                                     "SslMode=Preferred;";

                builder.Services.AddDbContext<DataContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configurando la base de datos: {ex.Message}");
                throw; // Re-throw para que Railway pueda ver el error
            }

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthorization();

            app.MapControllers();

            try
            {
                // Asegurar que la base de datos existe y está actualizada
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    db.Database.EnsureCreated(); // Esto creará la base de datos y las tablas
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando la base de datos: {ex.Message}");
                throw; // Re-throw para que Railway pueda ver el error
            }

            app.Run();
        }
    }
}
