using LibreriaApi.Models.Entities;
using System.Data.SqlClient;
using LibreriaApi.Repositories.Interfaces;

namespace LibreriaApi.Repositories
{
    public class LibroRepository : ILibroRepository
    {
        private readonly string _connectionString;

        public LibroRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<List<Libro>> ObtenerLibrosAsync()
        {
            var libros = new List<Libro>();
            const string query = @"
                SELECT Id, Titulo, ISBN, FechaPublicacion, Editorial, NumeroPaginas, 
                       Genero, Descripcion, FechaRegistro, Activo 
                FROM Libros 
                WHERE Activo = 1
                ORDER BY Titulo";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                libros.Add(MapearLibro(reader));
            }

            return libros;
        }

        public async Task<Libro?> ObtenerLibroPorIdAsync(int id)
        {
            const string query = @"
                SELECT Id, Titulo, ISBN, FechaPublicacion, Editorial, NumeroPaginas, 
                       Genero, Descripcion, FechaRegistro, Activo 
                FROM Libros 
                WHERE Id = @Id AND Activo = 1";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapearLibro(reader);
            }

            return null;
        }

        public async Task<int> CrearLibroAsync(Libro libro)
        {
            const string query = @"
                INSERT INTO Libros (Titulo, ISBN, FechaPublicacion, Editorial, NumeroPaginas, 
                                   Genero, Descripcion, FechaRegistro, Activo)
                VALUES (@Titulo, @ISBN, @FechaPublicacion, @Editorial, @NumeroPaginas, 
                        @Genero, @Descripcion, @FechaRegistro, @Activo);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            AgregarParametrosLibro(command, libro);

            await connection.OpenAsync();
            var resultado = await command.ExecuteScalarAsync();
            return Convert.ToInt32(resultado);
        }

        public async Task<bool> ActualizarLibroAsync(Libro libro)
        {
            const string query = @"
                UPDATE Libros 
                SET Titulo = @Titulo, ISBN = @ISBN, FechaPublicacion = @FechaPublicacion, 
                    Editorial = @Editorial, NumeroPaginas = @NumeroPaginas, 
                    Genero = @Genero, Descripcion = @Descripcion
                WHERE Id = @Id AND Activo = 1";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            AgregarParametrosLibro(command, libro);
            command.Parameters.AddWithValue("@Id", libro.Id);

            await connection.OpenAsync();
            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        public async Task<bool> EliminarLibroAsync(int id)
        {
            const string query = "UPDATE Libros SET Activo = 0 WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        public async Task<bool> AsignarAutorAsync(int libroId, int autorId)
        {
            const string query = @"
                IF NOT EXISTS (SELECT 1 FROM LibroAutor WHERE LibroId = @LibroId AND AutorId = @AutorId)
                INSERT INTO LibroAutor (LibroId, AutorId, FechaAsignacion)
                VALUES (@LibroId, @AutorId, @FechaAsignacion)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LibroId", libroId);
            command.Parameters.AddWithValue("@AutorId", autorId);
            command.Parameters.AddWithValue("@FechaAsignacion", DateTime.Now);

            await connection.OpenAsync();
            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        public async Task<List<Autor>> ObtenerAutoresPorLibroAsync(int libroId)
        {
            var autores = new List<Autor>();
            const string query = @"
                SELECT a.Id, a.Nombre, a.Apellido, a.FechaNacimiento, a.Nacionalidad, 
                       a.Bibliografia, a.FechaRegistro, a.Activo
                FROM Autores a
                INNER JOIN LibroAutor la ON a.Id = la.AutorId
                WHERE la.LibroId = @LibroId AND a.Activo = 1
                ORDER BY a.Apellido, a.Nombre";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LibroId", libroId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                autores.Add(MapearAutorFromReader(reader));
            }

            return autores;
        }

        public async Task<bool> RemoverAutorDeLibroAsync(int libroId, int autorId)
        {
            const string query = "DELETE FROM LibroAutor WHERE LibroId = @LibroId AND AutorId = @AutorId";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LibroId", libroId);
            command.Parameters.AddWithValue("@AutorId", autorId);

            await connection.OpenAsync();
            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        private static Libro MapearLibro(SqlDataReader reader)
        {
            return new Libro
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Titulo = reader.GetString(reader.GetOrdinal("Titulo")),
                ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                FechaPublicacion = reader.IsDBNull(reader.GetOrdinal("FechaPublicacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaPublicacion")),
                Editorial = reader.IsDBNull(reader.GetOrdinal("Editorial")) ? null : reader.GetString(reader.GetOrdinal("Editorial")),
                NumeroPaginas = reader.IsDBNull(reader.GetOrdinal("NumeroPaginas")) ? null : reader.GetInt32(reader.GetOrdinal("NumeroPaginas")),
                Genero = reader.IsDBNull(reader.GetOrdinal("Genero")) ? null : reader.GetString(reader.GetOrdinal("Genero")),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
            };
        }

        private static Autor MapearAutorFromReader(SqlDataReader reader)
        {
            return new Autor
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Apellido = reader.GetString(reader.GetOrdinal("Apellido")),
                FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                Nacionalidad = reader.IsDBNull(reader.GetOrdinal("Nacionalidad")) ? null : reader.GetString(reader.GetOrdinal("Nacionalidad")),
                Bibliografia = reader.IsDBNull(reader.GetOrdinal("Bibliografia")) ? null : reader.GetString(reader.GetOrdinal("Bibliografia")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
            };
        }

        private static void AgregarParametrosLibro(SqlCommand command, Libro libro)
        {
            command.Parameters.AddWithValue("@Titulo", libro.Titulo);
            command.Parameters.AddWithValue("@ISBN", (object?)libro.ISBN ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaPublicacion", (object?)libro.FechaPublicacion ?? DBNull.Value);
            command.Parameters.AddWithValue("@Editorial", (object?)libro.Editorial ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumeroPaginas", (object?)libro.NumeroPaginas ?? DBNull.Value);
            command.Parameters.AddWithValue("@Genero", (object?)libro.Genero ?? DBNull.Value);
            command.Parameters.AddWithValue("@Descripcion", (object?)libro.Descripcion ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaRegistro", libro.FechaRegistro);
            command.Parameters.AddWithValue("@Activo", libro.Activo);
        }
    }
}
