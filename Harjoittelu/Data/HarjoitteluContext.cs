using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Harjoittelu.Models;

namespace Harjoittelu.Data
{
    public class HarjoitteluContext : DbContext
    {
        public HarjoitteluContext (DbContextOptions<HarjoitteluContext> options)
            : base(options)
        {
        }

        public DbSet<Harjoittelu.Models.Student> Users { get; set; } = default!;
        public DbSet<Harjoittelu.Models.RFIDTag> Tags { get; set; } = default!;
        public DbSet<Harjoittelu.Models.LogInEvent> Loggings { get; set; } = default!;
    }
}
