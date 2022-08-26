using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class IzmeerDbContext: DbContext
    {
        DbSet<Customer> customers { get; set; }
        DbSet<Product> products { get; set; }
        DbSet<Answers> answers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=IzmeerProjectDb;Trusted_Connection=true");
        }
    }
}
