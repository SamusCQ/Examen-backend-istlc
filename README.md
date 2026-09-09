# API de usuarios en memoria

API desarrollada en C# con ASP.NET Core Minimal API y .NET 8.

El proyecto guarda los usuarios únicamente en memoria. No utiliza base de datos,
Entity Framework ni migraciones. Por ese motivo, los datos se reinician cuando se
detiene la aplicación.

También incluye Swagger UI para consultar y probar los endpoints desde el navegador.

## Requisitos

- .NET 8 SDK o una versión compatible.

Puedes confirmar la instalación con:

```bash
dotnet --version
```

## Ejecutar el proyecto

Desde la carpeta del proyecto ejecuta:

```bash
dotnet run
```

La consola mostrará la dirección donde quedó disponible la API, por ejemplo:

```text
http://localhost:5000
```

Al abrir `http://localhost:5000` se mostrará un mensaje indicando que la API
está funcionando y las rutas disponibles. Para consultar usuarios debes usar
la ruta `/api/usuarios`.

## Swagger

Con la aplicación ejecutándose, abre:

```text
http://localhost:5000/swagger
```

Desde allí puedes visualizar y ejecutar los endpoints `GET`, `POST` y `DELETE`
sin usar Postman. El documento OpenAPI también está disponible en:

```text
http://localhost:5000/swagger/v1/swagger.json
```

También puedes indicar una dirección fija:

```bash
dotnet run --urls http://localhost:5000
```

## Estructura

- `Program.cs`: configura la aplicación, registra el repositorio, muestra las peticiones en consola y define los endpoints.
- `Models/Usuario.cs`: contiene el modelo de usuario.
- `Dtos/CrearUsuarioRequest.cs`: contiene los datos permitidos para crear un usuario.
- `Repositories/IUsuarioRepository.cs`: define las operaciones de acceso a datos.
- `Repositories/UsuarioRepository.cs`: implementa el almacenamiento en una lista en memoria.

## Usuarios iniciales

Al iniciar la aplicación existen estos dos usuarios:

```json
[
  {
    "id": 1,
    "email": "ana@example.com",
    "nombre": "Ana",
    "apellido": "García"
  },
  {
    "id": 2,
    "email": "carlos@example.com",
    "nombre": "Carlos",
    "apellido": "Pérez"
  }
]
```

## Endpoints

### Obtener todos los usuarios

```http
GET /api/usuarios
```

Respuesta: `200 OK`.

PowerShell:

```powershell
Invoke-RestMethod -Method Get -Uri http://localhost:5000/api/usuarios
```

### Obtener un usuario por ID

```http
GET /api/usuarios/1
```

Respuesta: `200 OK` si existe o `404 Not Found` si no existe.

```powershell
Invoke-RestMethod -Method Get -Uri http://localhost:5000/api/usuarios/1
```

### Crear un usuario

```http
POST /api/usuarios
Content-Type: application/json
```

Body:

```json
{
  "email": "lucia@example.com",
  "nombre": "Lucía",
  "apellido": "Mendoza"
}
```

Respuesta: `201 Created`.

PowerShell:

```powershell
$body = @{
    email = "lucia@example.com"
    nombre = "Lucía"
    apellido = "Mendoza"
} | ConvertTo-Json

Invoke-RestMethod `
    -Method Post `
    -Uri http://localhost:5000/api/usuarios `
    -ContentType "application/json" `
    -Body $body
```

Si el correo ya existe, se devuelve `409 Conflict`. La comparación no distingue
mayúsculas y minúsculas. Por ejemplo, `ANA@EXAMPLE.COM` se considera repetido.

Si falta un campo o el correo no es válido, se devuelve `400 Bad Request`.

### Eliminar un usuario

```http
DELETE /api/usuarios/1
```

Respuesta: `204 No Content` si se eliminó o `404 Not Found` si no existe.

```powershell
Invoke-RestMethod -Method Delete -Uri http://localhost:5000/api/usuarios/1
```

## Registro en consola

Cada petición genera mensajes similares a estos:

```text
[2026-09-08 10:30:00] RECIBIDA: GET /api/usuarios
[2026-09-08 10:30:00] RESPUESTA: 200
```

Esto permite observar el método HTTP, la ruta y el resultado de cada petición.

## Probar el proyecto

Una secuencia sencilla es:

1. Ejecutar `dotnet run --urls http://localhost:5000`.
2. Consultar `GET /api/usuarios` y comprobar los dos registros iniciales.
3. Consultar `GET /api/usuarios/1`.
4. Consultar `GET /api/usuarios/99` y comprobar el `404`.
5. Crear un usuario con `POST` y comprobar el `201`.
6. Repetir el mismo correo y comprobar el `409`.
7. Enviar un correo inválido o dejar un campo vacío y comprobar el `400`.
8. Eliminar un usuario con `DELETE` y comprobar el `204`.
9. Intentar eliminar el mismo ID otra vez y comprobar el `404`.
