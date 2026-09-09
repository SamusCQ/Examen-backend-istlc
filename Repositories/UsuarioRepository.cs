using ExamenBackendApi.Models;

namespace ExamenBackendApi.Repositories;

/// <summary>
/// Repositorio en memoria.
/// Al detener la aplicación, todos los datos se pierden porque no se escribe en disco.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _usuarios =
    [
        new Usuario
        {
            Id = 1,
            Email = "ana@example.com",
            Nombre = "Ana",
            Apellido = "García"
        },
        new Usuario
        {
            Id = 2,
            Email = "carlos@example.com",
            Nombre = "Carlos",
            Apellido = "Pérez"
        }
    ];

    private int _siguienteId = 3;

    // List<T> no está diseñada para recibir escrituras simultáneas.
    // Este candado mantiene las operaciones seguras si llegan varias peticiones.
    private readonly object _candado = new();

    public IReadOnlyList<Usuario> ObtenerTodos()
    {
        lock (_candado)
        {
            // Devolvemos una copia para que quien consume el repositorio
            // no pueda modificar directamente la lista interna.
            return _usuarios.ToList();
        }
    }

    public Usuario? ObtenerPorId(int id)
    {
        lock (_candado)
        {
            return _usuarios.FirstOrDefault(usuario => usuario.Id == id);
        }
    }

    public Usuario? CrearSiCorreoDisponible(Usuario usuario)
    {
        lock (_candado)
        {
            var correoYaExiste = _usuarios.Any(actual =>
                string.Equals(actual.Email, usuario.Email, StringComparison.OrdinalIgnoreCase));

            if (correoYaExiste)
            {
                return null;
            }

            usuario.Id = _siguienteId++;
            _usuarios.Add(usuario);
            return usuario;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_candado)
        {
            var usuario = _usuarios.FirstOrDefault(actual => actual.Id == id);

            if (usuario is null)
            {
                return false;
            }

            _usuarios.Remove(usuario);
            return true;
        }
    }
}
