using System;
using System.Collections.Generic;
using System.Linq;
using OnlinePizzaWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace OnlinePizzaWebApplication.Data
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context, IServiceProvider service)
        {
            context.Database.EnsureCreated();

            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();

            if (context.Pizzas.Any())
            {
                return;
            }

            ClearDatabase(context);
            CreateAdminRole(context, roleManager, userManager);
            SeedDatabase(context, roleManager, userManager);
        }

        private static void CreateAdminRole(AppDbContext context, RoleManager<IdentityRole> _roleManager, UserManager<IdentityUser> _userManager)
        {
            bool roleExists = _roleManager.RoleExistsAsync("Admin").Result;
            if (roleExists)
            {
                return;
            }
            
            var role = new IdentityRole()
            {
                Name = "Admin"
            };
            _roleManager.CreateAsync(role).Wait();

            var user = new IdentityUser()
            {
                UserName = "admin",
                Email = "admin@default.com"
            };

            string adminPassword = "Password123";
            var userResult =  _userManager.CreateAsync(user, adminPassword).Result;

            if (userResult.Succeeded)
            {
                _userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }

        private static void SeedDatabase(AppDbContext _context, RoleManager<IdentityRole> _roleManager, UserManager<IdentityUser> _userManager)
        {
            var cat1 = new Categories { Name = "Standard" };
            var cat2 = new Categories { Name = "Spcialities" };
            var cat3 = new Categories { Name = "News" };

            var cats = new List<Categories>()
            {
                cat1, cat2, cat3
            }; 

            var piz1 = new Pizzas { Name = "מוצר א", Barcode = "123456", Weight="3 קג", Category = cat1, Description = "תיאור מוצר א", ImageUrl = "img/kedmaPic/IMG-20230312-WA0033.jpg" };

            var pizs = new List<Pizzas>()
            {
                piz1
            };

            var user1 = new IdentityUser { UserName = "user1@gmail.com", Email = "user1@gmail.com" };
            var user2 = new IdentityUser { UserName = "user2@gmail.com", Email = "user2@gmail.com" };
            var user3 = new IdentityUser { UserName = "user3@gmail.com", Email = "user3@gmail.com" };
            var user4 = new IdentityUser { UserName = "user4@gmail.com", Email = "user4@gmail.com" };
            var user5 = new IdentityUser { UserName = "user5@gmail.com", Email = "user5@gmail.com" };

            string userPassword = "Password123";

            var users = new List<IdentityUser>()
            {
                user1, user2, user3, user4, user5
            };

            foreach (var user in users)
            {
                _userManager.CreateAsync(user, userPassword).Wait();
            }

            var revs = new List<Reviews>()
            {
                new Reviews { User = user1, Title ="Too simple", Description="Too simple pizza, for such a high price.", Grade=2, Date=DateTime.Now.AddDays(-2), Pizza = piz1 },
                new Reviews { User = user5, Title ="Super Special", Description="Super special pizza, the best taste in the world!", Grade=5, Date=DateTime.Now.AddDays(-9), Pizza = piz1 },
            };

            var ing1 = new Ingredients { Name = "Cheese" };
            var ing2 = new Ingredients { Name = "Flour" };

            var ings = new List<Ingredients>()
            {
                ing1, ing2
            };

            var pizIngs = new List<PizzaIngredients>()
            {
                new PizzaIngredients { Ingredient = ing1, Pizza = piz1 },
                new PizzaIngredients { Ingredient = ing2, Pizza = piz1 }
            };

            var ord1 = new Order
            {
                FirstName = "Pelle",
                LastName = "Andersson",
                AddressLine1 = "MainStreet 12",
                City = "Gothenburg",
                Country = "Sweden",
                Email = "pelle22@gmail.com",
                OrderPlaced = DateTime.Now.AddDays(-2),
                PhoneNumber = "0705123456",
                User = user1,
                ZipCode = "43210",
                OrderTotal = 370,
            };

            var orderLines = new List<OrderDetail>()
            {
                new OrderDetail { Order=ord1, Pizza=piz1, AmountBoxes=2}
            };

            var orders = new List<Order>()
            {
                ord1
            };

            _context.Categories.AddRange(cats);
            _context.Pizzas.AddRange(pizs);
            _context.Reviews.AddRange(revs);
            _context.Orders.AddRange(orders);
            _context.OrderDetails.AddRange(orderLines);
            _context.Ingredients.AddRange(ings);
            _context.PizzaIngredients.AddRange(pizIngs);

            _context.SaveChanges();
        }

        private static void ClearDatabase(AppDbContext _context)
        {
            var pizzaIngredients = _context.PizzaIngredients.ToList();
            _context.PizzaIngredients.RemoveRange(pizzaIngredients);

            var ingredients = _context.Ingredients.ToList();
            _context.Ingredients.RemoveRange(ingredients);

            var reviews = _context.Reviews.ToList();
            _context.Reviews.RemoveRange(reviews);

            var shoppingCartItems = _context.ShoppingCartItems.ToList();
            _context.ShoppingCartItems.RemoveRange(shoppingCartItems);

            var users = _context.Users.ToList();
            var userRoles = _context.UserRoles.ToList();

            foreach (var user in users)
            {
                if (!userRoles.Any(r => r.UserId == user.Id))
                {
                    _context.Users.Remove(user);
                }
            }

            var orderDetails = _context.OrderDetails.ToList();
            _context.OrderDetails.RemoveRange(orderDetails);

            var orders = _context.Orders.ToList();
            _context.Orders.RemoveRange(orders);

            var pizzas = _context.Pizzas.ToList();
            _context.Pizzas.RemoveRange(pizzas);

            var categories = _context.Categories.ToList();
            _context.Categories.RemoveRange(categories);

            _context.SaveChanges();
        }
    }
}
