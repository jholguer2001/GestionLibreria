using LibreriaApi.Models.DTOs;

namespace LibreriaApi.Services.Interfaces
{
    public interface ILibroService
    {
        Task<List<LibroDTO>> ObtenerLibrosAsync();
        Task<LibroDTO?> ObtenerLibroPorIdAsync(int id);
        Task<LibroDTO> CrearLibroAsync(LibroDTO libroDto);
        Task<LibroDTO?> ActualizarLibroAsync(int id, LibroDTO libroDto);
        Task<bool> EliminarLibroAsync(int id);
        Task<bool> AsignarAutorAsync(int libroId, int autorId);
        Task<bool> RemoverAutorAsync(int libroId, int autorId);
        Task<bool> LibroDisponibleParaPrestamoAsync(int libroId);

    }
}
