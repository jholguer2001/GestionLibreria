using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models.DTOs
{
    public class PrestamoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del libro es obligatorio")]
        public int LibroId { get; set; }
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La fecha de devolución prevista es obligatoria")]
        public DateTime FechaDevolucionPrevista { get; set; }

        public DateTime? FechaDevolucionReal { get; set; }

        public string Estado { get; set; } = "Prestado";

        [Required(ErrorMessage = "El nombre de la persona es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string PrestadoA { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder los 500 caracteres")]
        public string? Comentarios { get; set; }

        public LibroDTO? Libro { get; set; }
    }
    public class RegistrarPrestamoDTO
    {
        [Required(ErrorMessage = "El ID del libro es obligatorio")]
        public int LibroId { get; set; }

        [Required(ErrorMessage = "El nombre de la persona es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string PrestadoA { get; set; } = string.Empty;

        [Range(1, 365, ErrorMessage = "El período de préstamo debe ser entre 1 y 365 días")]
        public int DiasPrestamo { get; set; } = 14;

        [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder los 500 caracteres")]
        public string? Comentarios { get; set; }
    }

    public class DevolucionPrestamoDTO
    {
        [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder los 500 caracteres")]
        public string? Comentarios { get; set; }
    }
}
