using Microsoft.EntityFrameworkCore;
using SplashShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Infrastructure
{
    public class SplashShoppingCartContext : DbContext
    {
        public SplashShoppingCartContext(DbContextOptions<SplashShoppingCartContext> options)
            : base(options)
        {
        }
        public DbSet<Page> Pages { get; set; }
    }
}
