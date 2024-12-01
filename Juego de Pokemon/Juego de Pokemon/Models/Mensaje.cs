using System.ComponentModel.DataAnnotations;

namespace Juego_de_Pokemon.Models
{
    public class Mensaje
    {
		public int Id { get; set; }
		public int RemitenteId { get; set; }
		public int DestinatarioId { get; set; }
		public string? Contenido { get; set; }
		public DateTime? FechaEnvio { get; set; }
		public string? Estado { get; set; }

		// Relaciones
		public Usuario? Remitente { get; set; }
		public Usuario? Destinatario { get; set; }
	}
}
