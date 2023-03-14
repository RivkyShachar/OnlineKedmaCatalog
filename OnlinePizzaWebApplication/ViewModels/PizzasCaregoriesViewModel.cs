using OnlinePizzaWebApplication.Models;
using System.Collections.Generic;

namespace OnlineKedmaWebApplication.ViewModels
{
    public class PizzasCaregoriesViewModel
    {
        public IEnumerable<Pizzas> Pizzas { get; set; }
        public IEnumerable<Categories> Categories { get; set; }
    }
}


