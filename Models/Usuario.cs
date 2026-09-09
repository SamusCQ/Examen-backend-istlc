namespace ExamenBackendApi.Models;

/// <summary>
/// Representa la información de un usuario.
/// Esta clase equivale a la estructura que normalmente se guardaría en una tabla,
/// pero en este proyecto se conserva únicamente en memoria.
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;
}
