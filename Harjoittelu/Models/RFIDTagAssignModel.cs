using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Harjoittelu.Models
{
    public class RFIDTagAssignModel
    {
        public List<SelectListItem>? students { get; set; }

        [Required]
        public int Tag_Id { get; set; }

        [Required]
        [MaxLength(24)]
        [Display(Name = "RFID Code")]
        public string Rfid_Id { get; set; }

        [MaxLength(48)]
        [Display(Name = "Serial Number")]
        public string? Serial { get; set; }

        [Display(Name = "Assigned to")]
        public int? StudentID { get; set; }
    }
}
