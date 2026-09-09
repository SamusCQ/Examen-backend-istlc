using ExamenBackendApi.Models;

namespace ExamenBackendApi.Repositories;

/// <summary>
/// Define las operaciones de acceso a usuarios.
/// La API depende de esta interfaz y no necesita conocer cómo se almacenan los datos.
/// </summary>
public interface IUsuarioRepository
{
    IReadOnlyList<Usuario> ObtenerTodos();

    Usuario? ObtenerPorId(int id);

    Usuario? CrearSiCorreoDisponible(Usuario usuario);

    bool Eliminar(int id);
}
