using LibreriaApi.Models.Entities;

namespace LibreriaApi.Repositories.Interfaces
{
    public interface ILibroRepository
    {
        Task<List<Libro>> ObtenerLibrosAsync();
        Task<Libro?> ObtenerLibroPorIdAsync(int id);
        Task<int> CrearLibroAsync(Libro libro);
        Task<bool> ActualizarLibroAsync(Libro libro);
        Task<bool> EliminarLibroAsync(int id);
        Task<bool> AsignarAutorAsync(int libroId, int autorId);
        Task<List<Autor>> ObtenerAutoresPorLibroAsync(int libroId);
        Task<bool> RemoverAutorDeLibroAsync(int libroId, int autorId);
    }
}
