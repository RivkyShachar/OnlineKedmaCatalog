using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaWebApplication.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }
        public Pizzas Pizza { get; set; }
        public int AmountBoxes { get; set; }
        public int AmountSingles { get; set; }
        public string ShoppingCartId { get; set; }
    }
}
