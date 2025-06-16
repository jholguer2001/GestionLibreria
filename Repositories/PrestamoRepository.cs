using LibreriaApi.Models.Entities;
using System.Data.SqlClient;
using LibreriaApi.Repositories.Interfaces;

namespace LibreriaApi.Repositories
{
    public class PrestamoRepository : IPrestamoRepository
    {
        private readonly string _connectionString;
        private readonly ILibroRepository _libroRepository;

        public PrestamoRepository(
            IConfiguration configuration,
            ILibroRepository libroRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            _libroRepository = libroRepository;
        }

        public async Task<List<Prestamo>> ObtenerPrestamosAsync(bool incluirDevueltos = false)
        {
            var prestamos = new List<Prestamo>();
            var query = @"
                SELECT Id, LibroId, FechaPrestamo, FechaDevolucionPrevista, 
                       FechaDevolucionReal, Estado, PrestadoA, Comentarios
                FROM Prestamos
                WHERE @IncluirDevueltos = 1 OR Estado IN ('Prestado', 'Atrasado')
                ORDER BY FechaPrestamo DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IncluirDevueltos", incluirDevueltos ? 1 : 0);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var prestamo = MapearPrestamo(reader);
                prestamo.Libro = await _libroRepository.ObtenerLibroPorIdAsync(prestamo.LibroId);
                prestamos.Add(prestamo);
            }

            return prestamos;
        }

        public async Task<Prestamo?> ObtenerPrestamoPorIdAsync(int id)
        {
            const string query = @"
                SELECT Id, LibroId, FechaPrestamo, FechaDevolucionPrevista, 
                       FechaDevolucionReal, Estado, PrestadoA, Comentarios
                FROM Prestamos
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var prestamo = MapearPrestamo(reader);
                prestamo.Libro = await _libroRepository.ObtenerLibroPorIdAsync(prestamo.LibroId);
                return prestamo;
            }

            return null;
        }

        public async Task<List<Prestamo>> ObtenerPrestamosPorLibroAsync(int libroId)
        {
            var prestamos = new List<Prestamo>();
            const string query = @"
                SELECT Id, LibroId, FechaPrestamo, FechaDevolucionPrevista, 
                       FechaDevolucionReal, Estado, PrestadoA, Comentarios
                FROM Prestamos
                WHERE LibroId = @LibroId
                ORDER BY FechaPrestamo DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LibroId", libroId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                prestamos.Add(MapearPrestamo(reader));
            }

            return prestamos;
        }

        public async Task<List<Prestamo>> ObtenerPrestamosPorEstadoAsync(string estado)
        {
            var prestamos = new List<Prestamo>();
            const string query = @"
                SELECT Id, LibroId, FechaPrestamo, FechaDevolucionPrevista, 
                       FechaDevolucionReal, Estado, PrestadoA, Comentarios
                FROM Prestamos
                WHERE Estado = @Estado
                ORDER BY FechaPrestamo DESC";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Estado", estado);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var prestamo = MapearPrestamo(reader);
                prestamo.Libro = await _libroRepository.ObtenerLibroPorIdAsync(prestamo.LibroId);
                prestamos.Add(prestamo);
            }

            return prestamos;
        }

        public async Task<int> RegistrarPrestamoAsync(Prestamo prestamo)
        {
            const string query = @"
                EXEC RegistrarPrestamo 
                    @LibroId, 
                    @PrestadoA, 
                    @DiasPrestamo, 
                    @Comentarios;
                SELECT SCOPE_IDENTITY();";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LibroId", prestamo.LibroId);
            command.Parameters.AddWithValue("@PrestadoA", prestamo.PrestadoA);
            command.Parameters.AddWithValue("@DiasPrestamo",
                (prestamo.FechaDevolucionPrevista - prestamo.FechaPrestamo).Days);
            command.Parameters.AddWithValue("@Comentarios",
                (object?)prestamo.Comentarios ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> RegistrarDevolucionAsync(int prestamoId)
        {
            const string query = "EXEC RegistrarDevolucion @PrestamoId";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PrestamoId", prestamoId);

            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> ActualizarPrestamoAsync(Prestamo prestamo)
        {
            const string query = @"
                UPDATE Prestamos
                SET FechaDevolucionPrevista = @FechaDevolucionPrevista,
                    Estado = @Estado,
                    Comentarios = @Comentarios
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", prestamo.Id);
            command.Parameters.AddWithValue("@FechaDevolucionPrevista", prestamo.FechaDevolucionPrevista);
            command.Parameters.AddWithValue("@Estado", prestamo.Estado);
            command.Parameters.AddWithValue("@Comentarios", (object?)prestamo.Comentarios ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        public async Task<bool> ExistePrestamoActivoAsync(int libroId)
        {
            const string query = @"
                SELECT 1 FROM Prestamos 
                WHERE LibroId = @LibroId AND Estado IN ('Prestado', 'Atrasado')";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LibroId", libroId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        private static Prestamo MapearPrestamo(SqlDataReader reader)
        {
            return new Prestamo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                LibroId = reader.GetInt32(reader.GetOrdinal("LibroId")),
                FechaPrestamo = reader.GetDateTime(reader.GetOrdinal("FechaPrestamo")),
                FechaDevolucionPrevista = reader.GetDateTime(reader.GetOrdinal("FechaDevolucionPrevista")),
                FechaDevolucionReal = reader.IsDBNull(reader.GetOrdinal("FechaDevolucionReal")) ?
                    null : reader.GetDateTime(reader.GetOrdinal("FechaDevolucionReal")),
                Estado = reader.GetString(reader.GetOrdinal("Estado")),
                PrestadoA = reader.GetString(reader.GetOrdinal("PrestadoA")),
                Comentarios = reader.IsDBNull(reader.GetOrdinal("Comentarios")) ?
                    null : reader.GetString(reader.GetOrdinal("Comentarios"))
            };
        }
    }
}