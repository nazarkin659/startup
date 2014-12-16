using GasBuddy.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasBuddy.Infrastructure
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext()
            : base("Local")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<ContactInfo> ContactInfo { get; set; }
    }
}
