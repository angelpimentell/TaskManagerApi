using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerApi;

public class CustomWebApplicationFactory : WebApplicationFactory<StartupMarker>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Puedes configurar entornos o servicios aquí si quieres
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development"); // o "Testing"

        // Puedes sobrescribir configuraciones, inyectar mocks, etc.
        builder.ConfigureServices(services =>
        {
            // Ejemplo: reemplazar un servicio real con uno falso para pruebas
            // services.AddSingleton<IMiServicio, MiServicioMock>();
        });
    }
}
