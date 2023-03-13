using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlinePizzaWebApplication.Data;
using OnlinePizzaWebApplication.Repositories;

namespace OnlinePizzaWebApplication.Controllers
{
    public class HomeController : Controller
    {
        // GET: Pizzas
        private readonly AppDbContext _context;
        private readonly IPizzaRepository _pizzaRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HomeController(AppDbContext context, IPizzaRepository pizzaRepo, ICategoryRepository categoryRepo)
        {
            _context = context;
            _pizzaRepo = pizzaRepo;
            _categoryRepo = categoryRepo;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _pizzaRepo.GetAllIncludedAsync());
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
