using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Juego_de_Pokemon.Models
{
    public class Enfermeria
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int PokemonId { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        public DateTime FechaPeticion { get; set; } = DateTime.Now;


        [ForeignKey("UsuarioId, PokemonId")]
        public virtual Usuario_Pokemones UsuarioPokemon { get; set; }
    }
}
