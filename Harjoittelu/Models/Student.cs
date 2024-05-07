using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace Harjoittelu.Models
{
    public class Student
    {
        [Flags]
        public enum Status_t : byte
        {
            LoggedIn = 1 << 0,
            AutoLogOut = 1 << 1,
            LoggedByAdmin = 1 << 2
        }

        [Key] public int Id { get; set; }

        [Required]
        [MaxLength(400)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^(fi|no|se)$", ErrorMessage = "Must be \"no\", \"se\" or \"fi\".")]
        public string Country { get; set; }

        public byte Status { get; set; } = 0;

        public string ColorClass()
        {
            return ColorClass(Status);
        }

        // Wanted to put all the color coding for all the tables in one place,
        // but couldn't figure out a good place for it, so I put it here.
        static public string ColorClass(byte status)
        {
            if ((status & (byte)Status_t.LoggedIn) != 0)
            {
                return "bg-success text-white";
            }
            else if (status == 0)
            {
                return "bg-warning";
            }
            else
            {
                return "bg-danger text-white";
            }
        }
    }
}
