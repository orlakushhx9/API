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

            // Configuración de la base de datos MySQL usando variables de entorno de Railway
            var connectionString = $"Server={Environment.GetEnvironmentVariable("MYSQLHOST")};" +
                                 $"Port={Environment.GetEnvironmentVariable("MYSQLPORT")};" +
                                 $"Database={Environment.GetEnvironmentVariable("MYSQL_DATABASE")};" +
                                 $"User={Environment.GetEnvironmentVariable("MYSQLUSER")};" +
                                 $"Password={Environment.GetEnvironmentVariable("MYSQLPASSWORD")};";

            builder.Services.AddDbContext<DataContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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

            // Asegurar que la base de datos existe y está actualizada
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureCreated(); // Esto creará la base de datos y las tablas
            }

            app.Run();
        }
    }
}
