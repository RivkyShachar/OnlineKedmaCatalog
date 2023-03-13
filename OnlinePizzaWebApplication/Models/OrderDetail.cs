using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaWebApplication.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int PizzaId { get; set; }
        public int AmountBoxes { get; set; }
        public int AmountSingles { get; set; }
        public float Price { get; set; }
        public virtual Pizzas Pizza { get; set; }
        public virtual Order Order { get; set; }
    }
}
