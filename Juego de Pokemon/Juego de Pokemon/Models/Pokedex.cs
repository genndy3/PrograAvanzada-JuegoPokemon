using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Juego_de_Pokemon.Models
{
    public class Pokedex
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        // Relaciones
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        public ICollection<PokedexPokemon> PokedexPokemones { get; set; }
    }
}
