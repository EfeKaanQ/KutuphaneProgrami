using System.Collections.Generic;

namespace KutuphaneProgrami.Models
{
    public class User
    {
        public User()
        {
            Books = new List<Book>();
            Loans = new List<Loan>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Book> Books { get; set; }
        public ICollection<Loan> Loans { get; set; }
    }
} 