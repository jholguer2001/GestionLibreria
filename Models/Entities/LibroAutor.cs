namespace LibreriaApi.Models.Entities
{
    public class LibroAutor
    {
        public int Id { get; set; }
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    }
}
