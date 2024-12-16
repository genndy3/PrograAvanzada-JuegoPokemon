namespace Juego_de_Pokemon.Models
{
	public class Batalla
	{
        public int Id { get; set; }
        public int Usuario1Id { get; set; }
        public int Usuario2Id { get; set; }

        public int PokemonUsuario1Id { get; set; }
        public int PokemonUsuario2Id { get; set; }

        public int GanadorId { get; set; }
        public DateTime? Fecha { get; set; }

        // Relaciones
        public Usuario? Usuario1 { get; set; }
        public Usuario? Usuario2 { get; set; }

        public Pokemon? PokemonUsuario1 { get; set; }
        public Pokemon? PokemonUsuario2 { get; set; }

        public Usuario? Ganador { get; set; }
    }
}
