using System.ComponentModel.DataAnnotations;

namespace Harjoittelu.Models
{
    public class RFIDTag
    {
        [Key]
        public int Tag_Id { get; set; }

        [Required]
        [MaxLength(24)]
        [Display(Name = "RFID Code")]
        public string Rfid_Id { get; set; }

        [MaxLength(48)]
        [Display(Name = "Serial Number")]
        public string? Serial { get; set; }

        public int? User_Id { get; set; }
    }
}
