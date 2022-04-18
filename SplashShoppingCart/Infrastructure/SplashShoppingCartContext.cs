using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Infrastructure
{
    public class SplashShoppingCartContext : IdentityDbContext<AppUser>
    {
        public SplashShoppingCartContext(DbContextOptions<SplashShoppingCartContext> options)
            : base(options)
        {
        }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
