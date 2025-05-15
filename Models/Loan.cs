using System;

namespace KutuphaneProgrami.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int? BookId { get; set; }
        public Book Book { get; set; }
        public int? StudentId { get; set; }
        public Student Student { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string StudentName { get; set; }
        public string StudentSurname { get; set; }
    }
} 