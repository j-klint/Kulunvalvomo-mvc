using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Harjoittelu.Models
{
    public class LogInEvent
    {
        [Key] public int Event_Id { get; set; }

        [Required]
        [ForeignKey(nameof(User_))]
        public int User_ID { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        public byte New_status { get; set; }

        // navigation property?
        public Student? User_ { get; set; } = null!;
    }
}
