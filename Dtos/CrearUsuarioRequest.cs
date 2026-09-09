namespace ExamenBackendApi.Dtos;

/// <summary>
/// Datos que el cliente debe enviar para crear un usuario.
/// El Id no se recibe porque lo genera automáticamente el repositorio.
/// </summary>
public class CrearUsuarioRequest
{
    public string? Email { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }
}
