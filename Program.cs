using System.ComponentModel.DataAnnotations;
using ExamenBackendApi.Dtos;
using ExamenBackendApi.HealthChecks;
using ExamenBackendApi.Models;
using ExamenBackendApi.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Registramos el repositorio como Singleton porque la información vive en memoria.
// Así, todos los endpoints comparten la misma lista mientras la aplicación está activa.
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();

// Registramos health checks para comprobar que la API está activa y que el
// repositorio en memoria puede leer correctamente la información.
builder.Services.AddHealthChecks()
    .AddCheck("api", () => HealthCheckResult.Healthy("La API está activa."))
    .AddCheck<MemoriaHealthCheck>("almacenamiento-en-memoria");

// Estas dos líneas permiten que Swagger descubra y documente los endpoints
// definidos con Minimal API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilita el documento OpenAPI y la interfaz visual de Swagger.
// Se dejan disponibles también fuera de Development para facilitar la revisión
// local del proyecto durante la exposición o la evaluación.
app.UseSwagger();
app.UseSwaggerUI();

// Este middleware se ejecuta para cada petición y muestra en la consola
// el método HTTP, la ruta y el código de respuesta generado.
app.Use(async (context, next) =>
{
    var fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    Console.WriteLine($"[{fechaHora}] RECIBIDA: {context.Request.Method} {context.Request.Path}");

    await next();

    Console.WriteLine($"[{fechaHora}] RESPUESTA: {context.Response.StatusCode}");
});

// Ruta de bienvenida para comprobar rápidamente que la API está activa.
// Las operaciones de usuarios continúan estando debajo de /api/usuarios.
app.MapGet("/", () => Results.Ok(new
{
    mensaje = "API de usuarios funcionando correctamente.",
    endpoints = new[]
    {
        "GET /api/usuarios",
        "GET /api/usuarios/{id}",
        "POST /api/usuarios",
        "DELETE /api/usuarios/{id}",
        "GET /health"
    }
}));

// GET /health
// Devuelve un resumen JSON del estado general de la API y de cada comprobación.
// Responde 200 cuando todo está saludable y 503 si alguna comprobación falla.
app.MapGet("/health", async (HealthCheckService servicioHealth) =>
{
    var reporte = await servicioHealth.CheckHealthAsync();
    var codigoHttp = reporte.Status == HealthStatus.Healthy
        ? StatusCodes.Status200OK
        : StatusCodes.Status503ServiceUnavailable;

    var comprobaciones = reporte.Entries.ToDictionary(
        entrada => entrada.Key,
        entrada => new
        {
            estado = entrada.Value.Status.ToString(),
            descripcion = entrada.Value.Description,
            datos = entrada.Value.Data
        });

    return Results.Json(new
    {
        estado = reporte.Status.ToString(),
        comprobaciones
    }, statusCode: codigoHttp);
});

// GET /api/usuarios
// Devuelve todos los usuarios registrados en memoria.
app.MapGet("/api/usuarios", (IUsuarioRepository repositorio) =>
{
    var usuarios = repositorio.ObtenerTodos();
    return Results.Ok(usuarios);
});

// GET /api/usuarios/{id}
// Busca un usuario por su identificador.
app.MapGet("/api/usuarios/{id:int}", (int id, IUsuarioRepository repositorio) =>
{
    var usuario = repositorio.ObtenerPorId(id);

    if (usuario is null)
    {
        return Results.NotFound(new
        {
            mensaje = $"No existe un usuario con el id {id}."
        });
    }

    return Results.Ok(usuario);
});

// POST /api/usuarios
// Crea un usuario nuevo después de validar sus datos.
app.MapPost("/api/usuarios", (CrearUsuarioRequest solicitud, IUsuarioRepository repositorio) =>
{
    var errores = ValidarSolicitud(solicitud);

    if (errores.Count > 0)
    {
        return Results.BadRequest(new
        {
            mensaje = "La información enviada no es válida.",
            errores
        });
    }

    // Trim elimina espacios accidentales al inicio o al final del texto.
    var usuario = new Usuario
    {
        Email = solicitud.Email!.Trim(),
        Nombre = solicitud.Nombre!.Trim(),
        Apellido = solicitud.Apellido!.Trim()
    };

    // El repositorio revisa el correo y la creación dentro de una misma operación.
    // Esto evita duplicados incluso si llegan dos peticiones al mismo tiempo.
    var usuarioCreado = repositorio.CrearSiCorreoDisponible(usuario);

    if (usuarioCreado is null)
    {
        return Results.Conflict(new
        {
            mensaje = $"Ya existe un usuario registrado con el correo '{usuario.Email}'."
        });
    }

    return Results.Created($"/api/usuarios/{usuarioCreado.Id}", usuarioCreado);
});

// DELETE /api/usuarios/{id}
// Elimina un usuario si existe.
app.MapDelete("/api/usuarios/{id:int}", (int id, IUsuarioRepository repositorio) =>
{
    var eliminado = repositorio.Eliminar(id);

    if (!eliminado)
    {
        return Results.NotFound(new
        {
            mensaje = $"No existe un usuario con el id {id}."
        });
    }

    return Results.NoContent();
});

app.Run();

// Valida manualmente la información recibida para que las respuestas sean
// claras y fáciles de entender desde Postman, Swagger o cualquier cliente HTTP.
static List<string> ValidarSolicitud(CrearUsuarioRequest solicitud)
{
    var errores = new List<string>();

    if (string.IsNullOrWhiteSpace(solicitud.Email))
    {
        errores.Add("El campo email es obligatorio.");
    }
    else if (!new EmailAddressAttribute().IsValid(solicitud.Email.Trim()))
    {
        errores.Add("El campo email debe tener un formato válido.");
    }

    if (string.IsNullOrWhiteSpace(solicitud.Nombre))
    {
        errores.Add("El campo nombre es obligatorio.");
    }

    if (string.IsNullOrWhiteSpace(solicitud.Apellido))
    {
        errores.Add("El campo apellido es obligatorio.");
    }

    return errores;
}
