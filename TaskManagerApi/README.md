# TaskManagerApi

TaskManager es un proyecto que ofrece una API para adminitrar tareas. Este proyecto está desarrollado utilizando .NET 9. A continuación se detallan los pasos para configurar, compilar y ejecutar el entorno de desarrollo en tu máquina local.

## Requisitos

- .NET 9 SDK: https://dotnet.microsoft.com/download/dotnet/9.0
- Visual Studio Code
- Git
- SQL Server

## Instalación

1. Descargar el SDK de .NET 9

   Descarga e instala el SDK desde la página oficial: https://dotnet.microsoft.com/download/dotnet/9.0

   Para verificar la instalación:
```bash
   dotnet --version
 ```

2. Clonar el repositorio
```bash
   git clone https://github.com/angelpimentell/TaskManagerApi.git
   cd TaskManagerApi
 ```
3. Restaurar los paquetes
```bash
   dotnet restore
 ```
4. Compilar el proyecto
```bash
   dotnet build
 ```
5. Ejecutar la aplicación
```bash
   dotnet run --project TaskManagerApi
 ```
   Asegúrate de reemplazar la ruta con el nombre correcto del proyecto si es necesario.

## Configuración

### Variables de entorno

Puedes configurar las variables de entorno necesarias en un archivo .env o directamente en tu entorno de desarrollo.

Ejemplo:

```bash
Server=localhost,1433;Database=task_manager;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
```

### Configuración de la base de datos

Usando Entity Framework Core aplica las migraciones con Package Manager Console:

```bash
PM > Update-Database
```
## Estructura del proyecto
```bash
/TaskManagerApi
├── Attributes/              # Validaciones personalizadas (ej. FutureDateAttribute)
├── Controllers/
│   └── Tasks/               # Lógica de control para tareas
├── Data/                    # Contexto de base de datos
├── Migrations/              # Migraciones de Entity Framework
├── Models/
│   └── Tasks/               # Modelos relacionados a tareas
├── Program.cs               # Configuración de la aplicación
├── TaskManagerApi.sln       # Solucion principal
```
## Licencia

Este proyecto está licenciado bajo la licencia MIT.
