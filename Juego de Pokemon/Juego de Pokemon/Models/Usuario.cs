using System.ComponentModel.DataAnnotations;

namespace Juego_de_Pokemon.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "La cuenta de usuario es obligatorio")]
        public required string CuentaUsuario { get; set; }
        
        public string? ContraseñaHash  { get; set; }
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "El rol de usuario es obligatorio")]
        public int RolId { get; set; }
    }
}
