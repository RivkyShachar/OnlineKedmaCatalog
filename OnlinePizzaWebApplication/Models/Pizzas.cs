using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaWebApplication.Models
{
    public class Pizzas
    {
        public Pizzas()
        {
            PizzaIngredients = new HashSet<PizzaIngredients>();
            Reviews = new HashSet<Reviews>();
        }

        public int Id { get; set; }

        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        public decimal Barcode { get; set; }

        [DataType(DataType.Text)]
        public string Weight { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        
        [DisplayName("Select Category")]
        public int CategoriesId { get; set; }

        public virtual Categories Category { get; set; }

        public virtual ICollection<PizzaIngredients> PizzaIngredients { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }

    }
}
