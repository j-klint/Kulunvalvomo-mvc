using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Harjoittelu.Models
{
    public class StudentViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(400)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^(fi|no|se)$", ErrorMessage = "Must be \"no\", \"se\" or \"fi\".")]
        public string Country { get; set; }

        public byte Status { get; set; } = 0;

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public TimeSpan? time { get; set; } = null;
        public List<LogInEvent> Events { get; set; } = new();

        public string Hours
        {
            get
            {
                if (time != null)
                    return $"{24 * time?.Days + time?.Hours} h {time?.Minutes} min";
                else
                    return "(start and end dates unspecified)";
            }
        }

        public StudentViewModel(Student s)
        {
            Id = s.Id;
            Name = s.Name;
            Country = s.Country;
            Status = s.Status;
        }

        // Jostain syystä pitää olla default-konstruktööri (parametriton), muuten ei model binding onnistu.
        // Ei haittaa vaikka visual studio ulisee non-nullable arvoista :D
        public StudentViewModel() { }
    }
}
