using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KutuphaneProgrami.Models
{
    public class Book
    {
        public Book()
        {
            Loans = new List<Loan>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public bool IsAvailable { get; set; } = true;
        [Required]
        public int BookTypeId { get; set; }
        public BookType? BookType { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Loan> Loans { get; set; }
    }
} 