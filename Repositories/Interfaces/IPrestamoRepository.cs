using LibreriaApi.Models.Entities;

namespace LibreriaApi.Repositories.Interfaces
{
    public interface IPrestamoRepository
    {
        Task<List<Prestamo>> ObtenerPrestamosAsync(bool incluirDevueltos = false);
        Task<Prestamo?> ObtenerPrestamoPorIdAsync(int id);
        Task<List<Prestamo>> ObtenerPrestamosPorLibroAsync(int libroId);
        Task<List<Prestamo>> ObtenerPrestamosPorEstadoAsync(string estado);
        Task<int> RegistrarPrestamoAsync(Prestamo prestamo);
        Task<bool> RegistrarDevolucionAsync(int prestamoId);
        Task<bool> ActualizarPrestamoAsync(Prestamo prestamo);
        Task<bool> ExistePrestamoActivoAsync(int libroId);
    }
}
