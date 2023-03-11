using Microsoft.EntityFrameworkCore;
using PrsBackEnd.Models;

namespace PrsBackEnd.Data
{
    public class PrsDbContext : DbContext  // not  a poco
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<Product> Product { get; set; }

        // constructor to support dependency injection (via a service)
        public PrsDbContext(DbContextOptions<PrsDbContext> options) : base(options)         // constructor - public, returns nothing, same name as class
        {
        }
        // constructor to support dependency injection (via a service)
        public DbSet<PrsBackEnd.Models.Request> Request { get; set; }


        // constructor to support dependency injection (via a service)
        public DbSet<PrsBackEnd.Models.RequestLine> RequestLine { get; set; }

    }
}
