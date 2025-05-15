using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KutuphaneProgrami.Models
{
    public class Student
    {
        public Student()
        {
            Loans = new List<Loan>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Number { get; set; }
        [Required]
        public int ClassId { get; set; }
        public Class? Class { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Loan> Loans { get; set; }
    }
} 