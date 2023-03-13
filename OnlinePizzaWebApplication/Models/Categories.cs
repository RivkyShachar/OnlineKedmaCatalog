using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlinePizzaWebApplication.Models
{
    public class Categories
    {
        public Categories()
        {
            Pizzas = new HashSet<Pizzas>();
        }

        public int Id { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]

        public virtual ICollection<Pizzas> Pizzas { get; set; }

    }
}