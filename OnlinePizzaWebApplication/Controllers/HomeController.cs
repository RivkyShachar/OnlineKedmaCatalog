using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OnlineKedmaWebApplication.ViewModels;
using OnlinePizzaWebApplication.Data;
using OnlinePizzaWebApplication.Models;
using OnlinePizzaWebApplication.Repositories;
using OnlinePizzaWebApplication.ViewModels;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net;

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

            var viewModel = new PizzasCaregoriesViewModel
            {
                Categories = await _categoryRepo.GetAllAsync(),
                Pizzas = await _context.Pizzas.Include(p => p.Category).Select(p => p).OrderBy(p => p.Category).ToListAsync()

            };
            return View(viewModel);
        }
        public async Task<IActionResult> CategoryA(int categoryId)
        {
            var pizzas = await _pizzaRepo.GetAllIncludedAsync();
            var pizzasSome= await _context.Pizzas.Where(p => p.CategoriesId == categoryId)
    .ToListAsync();
            //var pizzasSome = pizzas;//Where(p=>p.CategoriesId==categoryId);
            var viewModel = new PizzasCaregoriesViewModel
            {
                Categories = await _categoryRepo.GetAllAsync(),
                Pizzas = pizzasSome
            };
            return View(viewModel);
        }

        public async Task<IActionResult> About()
        {
            return View(await _pizzaRepo.GetAllIncludedAsync());
        }


        public async Task<IActionResult> Categories()
        {
            return View(await _categoryRepo.GetAllAsync());
        }

        public async Task<IActionResult> ShowCategories()
        {
            return View(await _categoryRepo.GetAllAsync());
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        // GET: Pizzas/Create
        public IActionResult Create()
        {
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Pizzas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Barcode,AmountInBox,ImageUrl,CategoriesId")] Pizzas pizzas)
        {
            if (ModelState.IsValid)
            {
                _pizzaRepo.Add(pizzas);
                await _pizzaRepo.SaveChangesAsync();
                return RedirectToAction("About");
            }
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", pizzas.CategoriesId);
            return View(pizzas);
        }

        // GET: Pizzas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizzas = await _pizzaRepo.GetByIdAsync(id);

            if (pizzas == null)
            {
                return NotFound();
            }
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", pizzas.CategoriesId);
            return View(pizzas);
        }

        // POST: Pizzas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Barcode,AmountInBox,ImageUrl,CategoriesId")] Pizzas pizzas)
        {
            if (id != pizzas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _pizzaRepo.Update(pizzas);
                    await _pizzaRepo.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PizzasExists(pizzas.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("About");
            }
            ViewData["CategoriesId"] = new SelectList(_categoryRepo.GetAll(), "Id", "Name", pizzas.CategoriesId);
            return View(pizzas);
        }

        // GET: Pizzas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizzas = await _pizzaRepo.GetByIdIncludedAsync(id);

            if (pizzas == null)
            {
                return NotFound();
            }

            return View(pizzas);
        }

        // POST: Pizzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pizzas = await _pizzaRepo.GetByIdAsync(id);
            _pizzaRepo.Remove(pizzas);
            await _pizzaRepo.SaveChangesAsync();
            return RedirectToAction("About");
        }

        // GET: Pizzas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizzas = await _pizzaRepo.GetByIdIncludedAsync(id);

            if (pizzas == null)
            {
                return NotFound();
            }

            return View(pizzas);
        }

        // GET: Pizzas/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> DisplayDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pizzas = await _pizzaRepo.GetByIdIncludedAsync(id);

            var listOfIngredients = await _context.PizzaIngredients.Where(x => x.PizzaId == id).Select(x => x.Ingredient.Name).ToListAsync();
            ViewBag.PizzaIngredients = listOfIngredients;

            //var listOfReviews = await _context.Reviews.Where(x => x.PizzaId == id).Select(x => x).ToListAsync();
            //ViewBag.Reviews = listOfReviews;
            double score;
            if (_context.Reviews.Any(x => x.PizzaId == id))
            {
                var review = _context.Reviews.Where(x => x.PizzaId == id);
                score = review.Average(x => x.Grade);
                score = Math.Round(score, 2);
            }
            else
            {
                score = 0;
            }
            ViewBag.AverageReviewScore = score;

            if (pizzas == null)
            {
                return NotFound();
            }

            return View(pizzas);
        }

        private bool PizzasExists(int id)
        {
            return _pizzaRepo.Exists(id);
        }

















        public async Task<IActionResult> DetailsCategories(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepo.GetByIdAsync(id);

            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult CreateCategories()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategories([Bind("Id,Name")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(categories);
                await _categoryRepo.SaveChangesAsync();
                return RedirectToAction("Categories");
            }
            return View(categories);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> EditCategories(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepo.GetByIdAsync(id);

            if (categories == null)
            {
                return NotFound();
            }
            return View(categories);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategories(int id, [Bind("Id,Name")] Categories categories)
        {
            if (id != categories.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _categoryRepo.Update(categories);
                    await _categoryRepo.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriesExists(categories.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Categories");
            }
            return View(categories);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> DeleteCategories(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepo.GetByIdAsync(id);

            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("DeleteCategories")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedCategories(int id)
        {
            var categories = await _categoryRepo.GetByIdAsync(id);
            _categoryRepo.Remove(categories);
            await _categoryRepo.SaveChangesAsync();

            return RedirectToAction("Categories");
        }

        private bool CategoriesExists(int id)
        {
            return _categoryRepo.Exists(id);
        }

        [AllowAnonymous]
        public IActionResult Password(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        //public void sendEmail()
        //{
        //    var attachment = new Attachment(new MemoryStream(pdfBytes), PdfFileName); //this is from the articles referred above

        //    MailMessage mailMessage = new MailMessage();
        //    mailMessage.To.Add(new MailAddress("rvider@g.jct.ac.il"));
        //    mailMessage.From = new MailAddress("rvider@g.jct.ac.il");
        //    mailMessage.Subject = "my pdf attached";
        //    mailMessage.Attachments.Add(attachment);

        //    SmtpClient smtp = new SmtpClient();
        //    smtp.Connect("smtp.gmail.com", 465, true);
        //    smtp.Authenticate("emailAddress", "Pwd");
        //    smtp.Send(mailMessage);
        //}
        public IActionResult SendEmailWithAttachment()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sender Name", "sender@example.com"));
            message.To.Add(new MailboxAddress("Recipient Name", "recipient@example.com"));
            message.Subject = "Email Subject";

            var pdfAttachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(System.IO.File.OpenRead("C:/Users/rivky/others Rivky/Software Developer-Rivka Shachar (3).pdf")),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                FileName = "filename.pdf"
            };

            var multipart = new Multipart("mixed");
            multipart.Add(pdfAttachment);
            message.Body = multipart;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("yaeli12131415@gmail.com", "088582728");
                client.Send(message);
                client.Disconnect(true);
            }
            return Content("Email sent successfully");
        }

        [HttpPost]
        public IActionResult SendEmail()
        {

            string filePath = "C:/Users/rivky/others Rivky/Software Developer-Rivka Shachar (3).pdf";
            // Create a System.Net.Mail.MailMessage object
            MailMessage message = new MailMessage();

            // Add a recipient
            message.To.Add("rvider@g.jct.ac.il");


            // Add a message subject
            message.Subject = "Account activation";

            // Add a message body
            message.Body = "Hello customer" + "\n" +
                "Welcome to Kedma.\n" +
                "We are so happy you have chosen to join us  and  are sure you will enjoy our services.\n" +
                "For any question, problems or requests you can contact our customer service team " +
                "at customerService@DroneDrop.com and they will gladly assist you.\n\n" +
                "Thank you and have a great day,\n" +
                "Team DroneDrop";

            // Create a System.Net.Mail.MailAddress object and 
            // set the sender email address and display name.
            message.From = new MailAddress("dronedrop2021@gmail.com", "DroneDrop");

            // Attach the file
            Attachment attachment = new Attachment(filePath);
            message.Attachments.Add(attachment);

            // Create a System.Net.Mail.SmtpClient object
            // and set the SMTP host and port number
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);

            // If your server requires authentication add the below code
            // =========================================================
            // Enable Secure Socket Layer (SSL) for connection encryption
            smtp.EnableSsl = true;

            // Do not send the DefaultCredentials with requests
            smtp.UseDefaultCredentials = false;

            // Create a System.Net.NetworkCredential object and set
            // the username and password required by your SMTP account
            smtp.Credentials = new NetworkCredential("dronedrop2021@gmail.com", "ouiatjczzhkvtxnr");
            // =========================================================

            // Send the message
            smtp.Send(message);

            // Redirect to the previous page
            //return RedirectToAction("Index");
            return Content("Email sent successfully");
        }

    }

}
