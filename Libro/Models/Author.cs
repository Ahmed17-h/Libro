namespace Libro.Models
{
    public class Author
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? ImageUrl { get; set; }

        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? WebsiteUrl { get; set; }

        public ICollection<Book>? Books { get; set; }
    }
}
