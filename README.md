# Sistema de Gestión de Librería

Una aplicación web integral diseñada para la gestión eficiente y moderna de libros y autores, con funcionalidades robustas para préstamos bibliográficos.

## Acerca del Proyecto

Este proyecto es un Sistema de Librería completo, desarrollado como una aplicación web integral para la gestión eficiente de inventarios bibliográficos. Permite a los usuarios administrar autores y libros, así como registrar y gestionar préstamos, ofreciendo una experiencia moderna y accesible. Su diseño modular asegura escalabilidad y fácil mantenimiento.

## Características Clave

* **Gestión Integral:** Aplicación web completa para la gestión de libros y autores, permitiendo un manejo eficiente de inventarios bibliográficos.
* **Funcionalidades CRUD:** Gestión completa (Crear, Leer, Actualizar, Eliminar) de autores y libros.
* **Gestión de Préstamos:** Permite registrar préstamos de libros, controlar la fecha de devolución prevista y registrar la devolución real, incluyendo verificación de disponibilidad.
* **Relaciones Múltiples:** Un libro puede tener varios autores, gestionado a través de una tabla intermedia.
* **Interfaz Moderna y Accesible:**
    * Interfaz web responsive, adaptable a diferentes dispositivos.
    * Diseño moderno utilizando CSS Grid y Flexbox.
    * Accesibilidad prioritaria con contraste adecuado, navegación clara e iconos significativos.
    * Búsqueda en tiempo real, notificaciones automáticas y validaciones de formularios.
* **API RESTful Documentada:** Backend con una API REST documentada (potencialmente con Swagger/OpenAPI si se añade en el futuro).
* **Acceso Flexible:** Acceso local y remoto por IP (requiere configuración), sin dependencias externas en el frontend.
* **Fácil Instalación y Mantenimiento:** Arquitectura modular y código bien estructurado.

## Arquitectura del Sistema

El sistema sigue una arquitectura cliente-servidor con una clara separación entre el Frontend (capa de presentación), el Backend (lógica de negocio y API) y la Base de Datos.

### Backend

Desarrollado con **ASP.NET Core Web API (C#)**, siguiendo principios de Clean Architecture o una estructura similar, con capas de Repositorios y Servicios. Utiliza **ADO.NET** para la interacción directa con la base de datos SQL Server, asegurando un control granular sobre las operaciones de datos.

### Frontend

Implementado con tecnologías web estándar: **HTML5, CSS3 y JavaScript Vanilla**. Se enfoca en un diseño responsive y una experiencia de usuario intuitiva, utilizando CSS Grid y Flexbox para la maquetación y JavaScript para la interactividad y las peticiones a la API.

### Base de Datos

Se utiliza **SQL Server Express** para el almacenamiento de datos. El esquema incluye las siguientes entidades principales:

* **Autores:** Almacena información detallada de los autores (Nombre, Apellido, Fecha de Nacimiento, Nacionalidad, Biografía). Incluye campos de auditoría (`FechaRegistro`, `Activo` para eliminación lógica).
* **Libros:** Contiene los detalles de los libros (Título, ISBN, Fecha de Publicación, Editorial, Número de Páginas, Género, Descripción). También incluye campos de auditoría (`FechaRegistro`, `Activo`).
* **LibroAutor:** Una tabla intermedia para gestionar la relación muchos a muchos entre `Libros` y `Autores`, registrando la `FechaAsignacion`.
* **Prestamos:** Registra los detalles de cada préstamo de libro (Libro prestado, Fecha de Préstamo, Fecha de Devolución Prevista, Fecha de Devolución Real, Estado, Persona a quien se prestó, Comentarios).

Para las operaciones de préstamo y devolución, se utilizan **Procedimientos Almacenados** (`RegistrarPrestamo`, `RegistrarDevolucion`) en la base de datos para garantizar la integridad y la atomnicidad de las transacciones.

## Tecnologías Utilizadas

* **Backend:**
    * C#
    * ASP.NET Core 8.0 Web API
    * ADO.NET
    * SQL Server (para la base de datos)
* **Frontend:**
    * HTML5
    * CSS3 (Flexbox, CSS Grid)
    * JavaScript (Vanilla JS)
* **Herramientas de Desarrollo:**
    * Visual Studio Community 2022 (para el Backend)
    * Visual Studio Code (opcional, para el Frontend)
    * SQL Server Management Studio (SSMS) (opcional, para gestionar la DB)

## Guía de Instalación y Uso

Sigue estos pasos para poner en marcha el sistema en tu entorno local.

### Requisitos Previos

Asegúrate de tener instalado lo siguiente:

* **Windows 10/11**
* **Visual Studio Community 2022** o superior
* **SQL Server Express** o SQL Server Developer Edition
* **.NET 8 SDK**
* **Visual Studio Code** (opcional, para el desarrollo frontend)
* Un navegador web moderno (Chrome, Firefox, Edge, etc.)
* **Python** (opcional, para servir el frontend localmente)

### Configuración de la Base de Datos

1.  **Crear la Base de Datos:** Abre SQL Server Management Studio (SSMS) y crea una nueva base de datos (por ejemplo, `LibreriaDB`).
2.  **Ejecutar Scripts SQL:** Ejecuta los scripts SQL para crear las tablas (`Autores`, `Libros`, `LibroAutor`, `Prestamos`) y los procedimientos almacenados (`RegistrarPrestamo`, `RegistrarDevolucion`). Estos scripts deberían estar disponibles en una carpeta `Database` o `SqlScripts` en el proyecto (si no lo están, deberás crearlos manualmente basándote en la estructura de entidades).

    *Ejemplo de estructura de tabla (simplificado, para referencia):*
    ```sql
    -- Tabla Autores
    CREATE TABLE Autores (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(100) NOT NULL,
        Apellido NVARCHAR(100) NOT NULL,
        FechaNacimiento DATE,
        Nacionalidad NVARCHAR(100),
        Bibliografia NVARCHAR(1000),
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        Activo BIT NOT NULL DEFAULT 1
    );

    -- Tabla Libros
    CREATE TABLE Libros (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Titulo NVARCHAR(200) NOT NULL,
        ISBN NVARCHAR(20),
        FechaPublicacion DATE,
        Editorial NVARCHAR(100),
        NumeroPaginas INT,
        Genero NVARCHAR(50),
        Descripcion NVARCHAR(1000),
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
        Activo BIT NOT NULL DEFAULT 1
    );

    -- Tabla LibroAutor (para la relación muchos a muchos)
    CREATE TABLE LibroAutor (
        Id INT PRIMARY KEY IDENTITY(1,1),
        LibroId INT NOT NULL,
        AutorId INT NOT NULL,
        FechaAsignacion DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (LibroId) REFERENCES Libros(Id),
        FOREIGN KEY (AutorId) REFERENCES Autores(Id),
        CONSTRAINT UQ_LibroAutor UNIQUE (LibroId, AutorId)
    );

    -- Tabla Prestamos
    CREATE TABLE Prestamos (
        Id INT PRIMARY KEY IDENTITY(1,1),
        LibroId INT NOT NULL,
        FechaPrestamo DATETIME NOT NULL DEFAULT GETDATE(),
        FechaDevolucionPrevista DATETIME NOT NULL,
        FechaDevolucionReal DATETIME,
        Estado NVARCHAR(20) NOT NULL DEFAULT 'Prestado', -- 'Prestado', 'Devuelto', 'Atrasado'
        PrestadoA NVARCHAR(100) NOT NULL,
        Comentarios NVARCHAR(500),
        FOREIGN KEY (LibroId) REFERENCES Libros(Id)
    );

    -- Procedimiento Almacenado para RegistrarPrestamo (ejemplo)
    CREATE PROCEDURE RegistrarPrestamo
        @LibroId INT,
        @PrestadoA NVARCHAR(100),
        @DiasPrestamo INT,
        @Comentarios NVARCHAR(500) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @FechaPrestamo DATETIME = GETDATE();
        DECLARE @FechaDevolucionPrevista DATETIME = DATEADD(day, @DiasPrestamo, @FechaPrestamo);

        INSERT INTO Prestamos (LibroId, FechaPrestamo, FechaDevolucionPrevista, PrestadoA, Comentarios, Estado)
        VALUES (@LibroId, @FechaPrestamo, @FechaDevolucionPrevista, @PrestadoA, @Comentarios, 'Prestado');

        SELECT SCOPE_IDENTITY() AS NewPrestamoId;
    END;

    -- Procedimiento Almacenado para RegistrarDevolucion (ejemplo)
    CREATE PROCEDURE RegistrarDevolucion
        @PrestamoId INT
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE Prestamos
        SET FechaDevolucionReal = GETDATE(),
            Estado = 'Devuelto'
        WHERE Id = @PrestamoId AND Estado IN ('Prestado', 'Atrasado');
    END;
    ```
3.  **Configurar Cadena de Conexión:** En el proyecto del Backend, abre el archivo `appsettings.json` y configura la cadena de conexión a tu base de datos:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=TU_SERVIDOR;Database=LibreriaDB;Integrated Security=True;TrustServerCertificate=True;"
        // O si usas autenticación SQL:
        // "DefaultConnection": "Server=TU_SERVIDOR;Database=LibreriaDB;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*"
    }
    ```
    Reemplaza `TU_SERVIDOR`, `TU_USUARIO` y `TU_PASSWORD` según tu configuración. Si usas SQL Server Express, `TU_SERVIDOR` podría ser `(localdb)\mssqllocaldb` o `.\SQLEXPRESS`.

### Configuración del Backend

1.  **Clonar el Repositorio:**
    ```bash
    git clone [https://github.com/tu-usuario/nombre-del-repositorio.git](https://github.com/tu-usuario/nombre-del-repositorio.git)
    cd nombre-del-repositorio/LibreriaApi
    ```
2.  **Abrir en Visual Studio:** Abre el archivo de solución `LibreriaApi.sln` en Visual Studio 2022.
3.  **Restaurar Paquetes NuGet:** Visual Studio debería restaurar automáticamente los paquetes NuGet. Si no, haz clic derecho en la solución en el Explorador de Soluciones y selecciona "Restaurar paquetes NuGet".
4.  **Compilar el Proyecto:** Compila el proyecto para asegurarte de que no haya errores.
5.  **Ejecutar el Backend:** Presiona `F5` en Visual Studio para iniciar el servidor de desarrollo del Backend. Esto normalmente abrirá la documentación de Swagger si está configurada (no se muestra en el código, pero es una práctica común). El API se ejecutará en un puerto local (ej. `https://localhost:7001`).

### Configuración del Frontend

El frontend consiste en archivos HTML, CSS y JavaScript. No requiere un proceso de compilación, pero necesita ser servido por un servidor web local para funcionar correctamente (evitar problemas de CORS y carga de archivos).

1.  **Crear Estructura de Archivos:** Asegúrate de tener los archivos `index.html`, `styles.css`, `script.js` (u otros archivos JS) en una carpeta dedicada para el frontend. Por ejemplo, dentro de una carpeta `Frontend` en la raíz de tu proyecto.
2.  **Servir con un Servidor HTTP Local:**
    * **Usando Python (recomendado para desarrollo):**
        Navega hasta la carpeta raíz de tu frontend en la terminal y ejecuta:
        ```bash
        python -m http.server 8000
        ```
        Esto iniciará un servidor en `http://localhost:8000`.
    * **Alternativas:** Puedes usar Live Server en VS Code o cualquier otro servidor web local.
3.  **Actualizar URL de la API:** Abre tus archivos JavaScript (ej. `script.js`) en la carpeta del Frontend y asegúrate de que la URL base de tu API esté apuntando a la dirección donde el Backend se está ejecutando (ej. `https://localhost:7001/api`).

    ```javascript
    // Ejemplo en tu script.js
    const API_BASE_URL = 'https://localhost:7001/api'; // Ajusta esto si tu backend usa otro puerto o dominio
    ```

### Acceso Remoto (Opcional)

Si necesitas acceder a la aplicación desde otros dispositivos en tu red:

1.  **Configurar `AllowedHosts` en `appsettings.json` (Backend):**
    Cambia `"AllowedHosts": "*"` para permitir cualquier IP, o especifica las IPs permitidas.
2.  **Actualizar URL de la API (Frontend):** En tu código JavaScript, cambia `localhost` por la dirección IP de la máquina donde se ejecuta el Backend (ej. `http://TU_IP_DEL_SERVIDOR:7001/api`).
3.  **Ajustar Firewall de Windows:** Asegúrate de que el puerto en el que se ejecuta tu Backend (ej. 7001 para HTTPS, 5001 para HTTP) esté abierto en el Firewall de Windows en la máquina del servidor.

## Estructura del Proyecto

La estructura del proyecto sigue una organización limpia y modular:
