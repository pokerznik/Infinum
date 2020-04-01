using Microsoft.EntityFrameworkCore;
using Infinum.ZanP.Core.Models.SQL;

namespace Infinum.ZanP.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {   
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<TelephoneNumber> TelephoneNumbers { get; set; }
    }
}