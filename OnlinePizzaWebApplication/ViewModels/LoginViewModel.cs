using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizzaWebApplication.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[ScaffoldColumn(false)]
        public string ReturnUrl { get; set; }
    }
}
