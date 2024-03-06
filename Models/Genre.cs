namespace TominoTrumpets.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public ICollection<Song>? Songs { get; set; }

    }
}


