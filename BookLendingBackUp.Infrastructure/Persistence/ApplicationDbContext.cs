using BookLendingBackUp.Domain.Entities;
using BookLendingBackUp.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<UsersBooksBorrow> UsersBooksBorrows { get; set; }

    }
}
