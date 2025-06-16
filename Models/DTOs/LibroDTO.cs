using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El ISBN no puede exceder los 20 caracteres")]
        public string? ISBN { get; set; }
        public DateTime? FechaPublicacion { get; set; }

        [StringLength(100, ErrorMessage = "La editorial no puede exceder los 100 caracteres")]
        public string? Editorial { get; set; }

        [Range(1, 10000, ErrorMessage = "El número de páginas debe estar entre 1 y 10000")]
        public int? NumeroPaginas { get; set; }

        [StringLength(50, ErrorMessage = "El género no puede exceder los 50 caracteres")]
        public string? Genero { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres")]
        public string? Descripcion { get; set; }

        public List<AutorDTO> Autores { get; set; } = new List<AutorDTO>();
    }
}
