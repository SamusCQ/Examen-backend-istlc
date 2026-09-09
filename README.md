# API de usuarios en memoria

API desarrollada en C# con ASP.NET Core Minimal API y .NET 8.

El proyecto guarda los usuarios únicamente en memoria. No utiliza base de datos,
Entity Framework ni migraciones. Por ese motivo, los datos se reinician cuando se
detiene la aplicación.

También incluye Swagger UI para consultar y probar los endpoints desde el navegador.

## Crear el proyecto desde cero en Visual Studio

Si vas a clonar este repositorio, puedes saltar esta sección porque el proyecto
ya está creado. Estos son los pasos para construirlo manualmente desde cero.

### 1. Crear el proyecto

1. Abre Visual Studio 2022.
2. Selecciona **Crear un nuevo proyecto**.
3. Busca y selecciona **ASP.NET Core Empty**.
   - Lenguaje: **C#**.
   - Tipo de proyecto: **Web**.
   - Esta plantilla es adecuada porque el proyecto utiliza **Minimal API** y no controladores.
4. Presiona **Siguiente**.
5. Configura el proyecto:
   - Nombre: `ExamenBackendApi`.
   - Ubicación: la carpeta donde guardarás el proyecto.
   - Solución: puede tener el mismo nombre `ExamenBackendApi`.
6. Presiona **Siguiente**.
7. En información adicional selecciona:
   - Framework: **.NET 8.0 (Long Term Support)**.
   - Tipo de autenticación: **Ninguno**.
   - **Configurar para HTTPS**: activado.
   - Docker: desactivado.
8. Presiona **Crear**.

La opción **Configurar para HTTPS** es la razón por la que Visual Studio puede
abrir una dirección parecida a `https://localhost:62565`. El número de puerto
puede cambiar en cada equipo.

### 2. Crear las carpetas

En el Explorador de soluciones, haz clic derecho sobre el proyecto y selecciona
**Agregar > Nueva carpeta**. Crea estas carpetas:

```text
Dtos
HealthChecks
Models
Repositories
```

### 3. Crear los archivos

Agrega los siguientes archivos usando **Agregar > Clase** y copia el contenido
correspondiente desde este repositorio:

```text
Dtos/CrearUsuarioRequest.cs
HealthChecks/MemoriaHealthCheck.cs
Models/Usuario.cs
Repositories/IUsuarioRepository.cs
Repositories/UsuarioRepository.cs
```

Después reemplaza el contenido de `Program.cs` con el archivo de este proyecto.
Ese archivo configura los endpoints, Swagger, health check, validaciones y
registro de peticiones en consola.

### 4. Instalar Swagger

Desde Visual Studio puedes instalar el paquete así:

1. Ve a **Herramientas > Administrador de paquetes NuGet > Administrar paquetes NuGet para la solución**.
2. Abre la pestaña **Examinar**.
3. Busca `Swashbuckle.AspNetCore`.
4. Selecciona el proyecto `ExamenBackendApi`.
5. Presiona **Instalar** y acepta las condiciones.

También puedes usar la consola de terminal ubicada en la carpeta del proyecto:

```powershell
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
```

El archivo `.csproj` debe contener una referencia parecida a esta:

```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

### 5. Restaurar, compilar y ejecutar

En Visual Studio:

1. Selecciona **Compilar > Compilar solución**.
2. Comprueba que no existan errores.
3. Presiona `Ctrl + F5` para ejecutar sin depuración o `F5` para ejecutar con depuración.

También puedes hacerlo desde una terminal:

```powershell
dotnet restore
dotnet build
dotnet run
```

Visual Studio abrirá la URL configurada en `Properties/launchSettings.json`.
Si aparece una dirección HTTPS como `https://localhost:62565`, usa ese mismo
puerto para las siguientes rutas.

### 6. Verificar que todo funciona

Con una URL de ejemplo `https://localhost:62565`, abre:

```text
https://localhost:62565/
https://localhost:62565/swagger
https://localhost:62565/health
https://localhost:62565/api/usuarios
```

La primera vez que uses HTTPS de forma local, el navegador puede mostrar una
advertencia sobre el certificado de desarrollo. Es normal en ASP.NET Core:
selecciona **Avanzado** y continúa únicamente si estás trabajando en tu equipo.

Si utilizas `dotnet run --urls http://localhost:5000`, las mismas rutas serán:

```text
http://localhost:5000/
http://localhost:5000/swagger
http://localhost:5000/health
http://localhost:5000/api/usuarios
```

### 7. Probar el health check

Abre `/health`. Una respuesta correcta debe mostrar `Healthy`, el estado de la
API y la cantidad de usuarios que están actualmente en memoria. Por ejemplo:

```json
{
  "estado": "Healthy",
  "comprobaciones": {
    "api": {
      "estado": "Healthy"
    },
    "almacenamiento-en-memoria": {
      "estado": "Healthy"
    }
  }
}
```

En este proyecto no se comprueba una base de datos porque la información se
guarda en una lista en memoria. Al cerrar la aplicación, los usuarios vuelven a
los dos registros iniciales.

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

## Health check

La API tiene un endpoint para revisar su estado y el estado del almacenamiento
en memoria:

```text
https://localhost:62565/health
```

Si ejecutas la aplicación con otra URL o puerto, reemplaza la parte inicial.
Por ejemplo, con `dotnet run --urls http://localhost:5000` usa:

```text
http://localhost:5000/health
```

Una respuesta saludable se parece a:

```json
{
  "estado": "Healthy",
  "comprobaciones": {
    "api": {
      "estado": "Healthy",
      "descripcion": "La API está activa."
    },
    "almacenamiento-en-memoria": {
      "estado": "Healthy",
      "descripcion": "El almacenamiento en memoria está disponible.",
      "datos": {
        "usuariosEnMemoria": 2
      }
    }
  }
}
```

Como no existe una base de datos en este proyecto, el segundo check valida el
repositorio en memoria en lugar de una conexión a base de datos.

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

## Diccionario de carpetas y conceptos

Esta sección explica con palabras sencillas para qué sirve cada parte del proyecto.

### `Dtos`

DTO significa **Data Transfer Object**, que en español significa **objeto para
transferir datos**. Aquí se define la información que el cliente puede enviar a
la API. Por ejemplo, `CrearUsuarioRequest` recibe `email`, `nombre` y `apellido`.
El `Id` no se recibe porque lo genera la aplicación.

### `HealthChecks`

Contiene las comprobaciones de estado de la aplicación. En este proyecto,
`MemoriaHealthCheck` verifica que el repositorio en memoria pueda leerse
correctamente. Como no usamos base de datos, no se comprueba una conexión a una
base de datos real.

### `Models`

Contiene los **modelos**, es decir, las clases que representan la información
principal del sistema. `Usuario.cs` define las propiedades `Id`, `Email`,
`Nombre` y `Apellido`.

### `Repositories`

Contiene la lógica para guardar y consultar datos. La interfaz
`IUsuarioRepository` define qué operaciones existen, mientras que
`UsuarioRepository` explica cómo se realizan usando una lista en memoria.
Esta separación permite cambiar posteriormente la lista por una base de datos
sin reescribir todos los endpoints.

### `Program.cs`

Es el archivo principal de la aplicación. Allí se configura el proyecto, se
registran las dependencias, se activa Swagger, se registra el health check, se
imprime cada petición en la consola y se definen las rutas HTTP.

### `ExamenBackendApi.csproj`

Es el archivo de configuración del proyecto C#. Indica que se usa .NET 8 y
contiene el paquete `Swashbuckle.AspNetCore`, necesario para Swagger.

### `Properties/launchSettings.json`

Define cómo Visual Studio inicia la aplicación. Allí se establece que se puede
usar HTTPS y se configura una dirección como `https://localhost:62565`.
El puerto puede ser diferente en otro equipo.

### `.gitignore`

Indica a Git qué archivos no debe subir, como `bin`, `obj` y `.vs`, porque son
archivos generados automáticamente por .NET o Visual Studio.

### Minimal API

Es un estilo de ASP.NET Core que permite crear endpoints con poco código,
directamente usando métodos como `app.MapGet`, `app.MapPost` y `app.MapDelete`.

### Endpoint

Es una dirección de la API que realiza una operación. Por ejemplo,
`GET /api/usuarios` obtiene todos los usuarios y `POST /api/usuarios` crea uno.

### Repositorio en memoria

Es la lista donde se guardan temporalmente los usuarios mientras la aplicación
está encendida. Al cerrar o reiniciar el programa, la lista vuelve a sus dos
usuarios iniciales porque no se utiliza una base de datos.

### Swagger

Es una interfaz web que muestra los endpoints disponibles y permite probarlos
sin utilizar Postman. Se abre en la ruta `/swagger`.

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
