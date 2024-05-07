using System.ComponentModel.DataAnnotations;

namespace Harjoittelu.Models
{
    public class RFIDTagViewModel
    {
        public int Tag_Id { get; set; }

        [Display(Name = "RFID Code")]
        public string? Rfid_Id { get; set; }

        [Display(Name = "Serial Number")]
        public string? Serial { get; set; }

        [Display(Name = "Assigned to")]
        public string? UserName { get; set; }
    }
}
