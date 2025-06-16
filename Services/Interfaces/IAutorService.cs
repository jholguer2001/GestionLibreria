using LibreriaApi.Models.DTOs;

namespace LibreriaApi.Services.Interfaces
{
    public interface IAutorService // Cambiado de "class" a "interface"
    {
        Task<List<AutorDTO>> ObtenerAutoresAsync();
        Task<AutorDTO?> ObtenerAutorPorIdAsync(int id);
        Task<AutorDTO> CrearAutorAsync(AutorDTO autorDto);
        Task<AutorDTO?> ActualizarAutorAsync(int id, AutorDTO autorDto);
        Task<bool> EliminarAutorAsync(int id);
    }
}
