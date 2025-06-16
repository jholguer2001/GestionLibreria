using LibreriaApi.Models.Entities;
using System.Data.SqlClient;
using LibreriaApi.Repositories.Interfaces;

namespace LibreriaApi.Repositories
{
    public class AutorRepository : IAutorRepository
    {
        private readonly string _connectionString;
        
        public AutorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        public async Task<List<Autor>> ObtenerAutoresAsync()
        {
            var autores = new List<Autor>();
            const string query = @"
                SELECT Id, Nombre, Apellido, FechaNacimiento, Nacionalidad, Bibliografia, FechaRegistro, Activo 
                FROM Autores 
                WHERE Activo = 1
                ORDER BY Apellido, Nombre";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                autores.Add(MapearAutor(reader));
            }

            return autores;
        }

        public async Task<Autor?> ObtenerAutorPorIdAsync(int id)
        {
            const string query = @"
                SELECT Id, Nombre, Apellido, FechaNacimiento, Nacionalidad, Bibliografia, FechaRegistro, Activo 
                FROM Autores 
                WHERE Id = @Id AND Activo = 1";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapearAutor(reader);
            }

            return null;
        }

        public async Task<int> CrearAutorAsync(Autor autor)
        {
            const string query = @"
                INSERT INTO Autores (Nombre, Apellido, FechaNacimiento, Nacionalidad, Bibliografia, FechaRegistro, Activo)
                VALUES (@Nombre, @Apellido, @FechaNacimiento, @Nacionalidad, @Bibliografia, @FechaRegistro, @Activo);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            AgregarParametrosAutor(command, autor);

            await connection.OpenAsync();
            var resultado = await command.ExecuteScalarAsync();
            return Convert.ToInt32(resultado);
        }

        public async Task<bool> ActualizarAutorAsync(Autor autor)
        {
            const string query = @"
                UPDATE Autores 
                SET Nombre = @Nombre, Apellido = @Apellido, FechaNacimiento = @FechaNacimiento, 
                    Nacionalidad = @Nacionalidad, Bibliografia = @Bibliografia
                WHERE Id = @Id AND Activo = 1";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            AgregarParametrosAutor(command, autor);
            command.Parameters.AddWithValue("@Id", autor.Id);

            await connection.OpenAsync();
            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        public async Task<bool> EliminarAutorAsync(int id)
        {
            const string query = "UPDATE Autores SET Activo = 0 WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        private static Autor MapearAutor(SqlDataReader reader)
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
        private static void AgregarParametrosAutor(SqlCommand command, Autor autor)
        {
            command.Parameters.AddWithValue("@Nombre", autor.Nombre);
            command.Parameters.AddWithValue("@Apellido", autor.Apellido);
            command.Parameters.AddWithValue("@FechaNacimiento", (object?)autor.FechaNacimiento ?? DBNull.Value);
            command.Parameters.AddWithValue("@Nacionalidad", (object?)autor.Nacionalidad ?? DBNull.Value);
            command.Parameters.AddWithValue("@Bibliografia", (object?)autor.Bibliografia ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaRegistro", autor.FechaRegistro);
            command.Parameters.AddWithValue("@Activo", autor.Activo);
        }
    }
}
