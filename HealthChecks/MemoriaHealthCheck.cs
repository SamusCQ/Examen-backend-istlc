using ExamenBackendApi.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ExamenBackendApi.HealthChecks;

/// <summary>
/// Comprueba que el repositorio en memoria está disponible y puede leer usuarios.
/// En este proyecto reemplaza la comprobación de conexión a base de datos,
/// porque la información no se guarda en una base de datos.
/// </summary>
public class MemoriaHealthCheck : IHealthCheck
{
    private readonly IUsuarioRepository _repositorio;

    public MemoriaHealthCheck(IUsuarioRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cantidadUsuarios = _repositorio.ObtenerTodos().Count;
            var datos = new Dictionary<string, object>
            {
                ["usuariosEnMemoria"] = cantidadUsuarios
            };

            return Task.FromResult(
                HealthCheckResult.Healthy("El almacenamiento en memoria está disponible.", datos));
        }
        catch (Exception excepcion)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy("No se pudo leer el almacenamiento en memoria.", excepcion));
        }
    }
}
