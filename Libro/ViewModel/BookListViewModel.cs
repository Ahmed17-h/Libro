using Libro.Models;

namespace Libro.ViewModel
{
    public class BookListViewModel
    {
        public List<Book> Books { get; set; }
        public string? SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}