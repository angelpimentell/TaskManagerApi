using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerApi;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<StartupMarker>
{


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Eliminar la configuración previa de AppDbContext con SQL Server
            services.Remove(
                services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>))
            );

            // Registrar base de datos en memoria para pruebas
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTestDb");
            });

            // Crear el service provider para inicializar la BD con datos de prueba
            var sp = services.BuildServiceProvider();

        });
    }

}
