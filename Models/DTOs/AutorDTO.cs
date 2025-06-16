using System.ComponentModel.DataAnnotations;

namespace LibreriaApi.Models.DTOs
{
    public class AutorDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(100, ErrorMessage = "La nacionalidad no puede exceder los 100 caracteres")]
        public string? Nacionalidad { get; set; }

        [StringLength(1000, ErrorMessage = "La biografía no puede exceder los 1000 caracteres")]
        public string? Bibliografia { get; set; }
    }
}
