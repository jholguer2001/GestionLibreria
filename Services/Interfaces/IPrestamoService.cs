using LibreriaApi.Models.DTOs;



namespace LibreriaApi.Services.Interfaces
{
    public interface IPrestamoService
    {

        Task<List<PrestamoDTO>> ObtenerTodosAsync(bool incluirDevueltos = false);
        Task<PrestamoDTO?> ObtenerPorIdAsync(int id);
        Task<List<PrestamoDTO>> ObtenerPorLibroAsync(int libroId);
        Task<List<PrestamoDTO>> ObtenerPorEstadoAsync(string estado);
        Task<PrestamoDTO> RegistrarPrestamoAsync(RegistrarPrestamoDTO prestamoDto);
        Task<bool> RegistrarDevolucionAsync(int prestamoId, DevolucionPrestamoDTO devolucionDto);
        Task<bool> ActualizarPrestamoAsync(int id, PrestamoDTO prestamoDto);
        Task<bool> LibroDisponibleAsync(int libroId);
    }
}
