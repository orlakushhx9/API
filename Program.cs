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

            // Configuración de la base de datos MySQL
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

            // Aplicar migraciones automáticamente
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}
