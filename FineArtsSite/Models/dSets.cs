using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FineArtsSite.Models
{
    public class DSets : DbContext
    {
        public DbSet<Inventory> Inventory { get; set; }
    }
}
