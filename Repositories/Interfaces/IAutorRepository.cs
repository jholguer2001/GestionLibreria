using LibreriaApi.Models.Entities;

namespace LibreriaApi.Repositories.Interfaces
{
    public interface IAutorRepository
    {
        Task<List<Autor>> ObtenerAutoresAsync();
        Task<Autor?> ObtenerAutorPorIdAsync(int id);
        Task<int> CrearAutorAsync(Autor autor);
        Task<bool> ActualizarAutorAsync(Autor autor);
        Task<bool> EliminarAutorAsync(int id);
    }
}
