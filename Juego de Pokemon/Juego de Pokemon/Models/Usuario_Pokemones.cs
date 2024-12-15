namespace Juego_de_Pokemon.Models
{
    public class Usuario_Pokemones
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PokemonId { get; set; }

        public Usuario Usuario { get; set; }
        public Pokemon Pokemon { get; set; }
    }

}
