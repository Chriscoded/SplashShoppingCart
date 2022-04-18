using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Models
{
    public class Transaction
    {
        public String Id { get; set; } = Guid.NewGuid().ToString();
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string TrxRef { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
