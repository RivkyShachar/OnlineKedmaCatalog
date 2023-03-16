using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlinePizzaWebApplication.Repositories;
using OnlinePizzaWebApplication.Models;
using OnlinePizzaWebApplication.ViewModels;
using OnlinePizzaWebApplication.Data;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace OnlinePizzaWebApplication.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IPizzaRepository _pizzaRepository;
        private readonly AppDbContext _context;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(IPizzaRepository pizzaRepository,
            ShoppingCart shoppingCart, AppDbContext context)
        {
            _pizzaRepository = pizzaRepository;
            _shoppingCart = shoppingCart;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _shoppingCart.GetShoppingCartItemsAsync();
            _shoppingCart.ShoppingCartItems = items;

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(shoppingCartViewModel);
        }

        public async Task<IActionResult> AddToShoppingCartBoxes(int pizzaId, int amount=1)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.AddToCartAsyncBoxes(selectedPizza, amount, amount);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddToShoppingCartSingles(int pizzaId, int amount = 1)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.AddToCartAsyncSingles(selectedPizza, amount, amount);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateToShoppingCartSingles(int pizzaId, int quantity)
        {
           var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.UpdateToCartAsyncSingles(selectedPizza, quantity);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdatePrice(int pizzaId, int price)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.UpdatePrice(selectedPizza, price);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UpdateToShoppingCartBoxes(int pizzaId, int quantityB)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.UpdateToCartAsyncBoxes(selectedPizza, quantityB);
            }
            return RedirectToAction("Index");
        }
      


        public async Task<IActionResult> AddToShoppingCart1(int pizzaId, int amount = 1)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.AddToCartAsync(selectedPizza, amount, amount);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> AddToShoppingCart2(int pizzaId, int categoryId, int amount = 1)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.AddToCartAsync(selectedPizza, amount, amount);
            }
            return RedirectToAction("CategoryA", "Home", new { categoryId = categoryId });
        }

        public async Task<IActionResult> RemoveFromShoppingCart(int pizzaId)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.RemoveFromCartAsync(selectedPizza);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateShoppingCart(int pizzaId, int amount)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.UpdateCartAsync(selectedPizza, amount, amount);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ClearCart()
        {
            await _shoppingCart.ClearCartAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Save()
        {
            var items = await _shoppingCart.GetShoppingCartItemsAsync();
            _shoppingCart.ShoppingCartItems = items;

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(shoppingCartViewModel);
        }

        //[HttpPost]
        //public IActionResult SendEmail()
        //{
        //    // Create a System.Net.Mail.MailMessage object
        //    MailMessage message = new MailMessage();

        //    // Add a recipient
        //    message.To.Add("ygloberm@g.jct.ac.il");

        //    // Add a message subject
        //    message.Subject = "Account activation";

        //    // Add a message body
        //    message.Body = "Hello customer" + "\n" +
        //        "Welcome to Kedma.\n" +
        //        "We are so happy you have chosen to join us  and  are sure you will enjoy our services.\n" +
        //        "For any question, problems or requests you can contact our customer service team " +
        //        "at customerService@DroneDrop.com and they will gladly assist you.\n\n" +
        //        "Thank you and have a great day,\n" +
        //        "Team DroneDrop";

        //    // Create a System.Net.Mail.MailAddress object and 
        //    // set the sender email address and display name.
        //    message.From = new MailAddress("dronedrop2021@gmail.com", "DroneDrop");

        //    // Create a System.Net.Mail.SmtpClient object
        //    // and set the SMTP host and port number
        //    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        //    // If your server requires authentication add the below code
        //    // =========================================================
        //    // Enable Secure Socket Layer (SSL) for connection encryption
        //    smtp.EnableSsl = true;

        //    // Do not send the DefaultCredentials with requests
        //    smtp.UseDefaultCredentials = false;

        //    // Create a System.Net.NetworkCredential object and set
        //    // the username and password required by your SMTP account
        //    smtp.Credentials = new NetworkCredential("dronedrop2021@gmail.com", "ouiatjczzhkvtxnr");
        //    // =========================================================

        //    // Send the message
        //    smtp.Send(message);

        //    // Redirect to the previous page
        //    return RedirectToAction("Index");
        //}
    


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
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

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
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SendEmail1(string pdfData)
        {
            if (pdfData == null)
            {
                return BadRequest("PDF data cannot be null.");
            }


            // Convert the PDF data from a Base64 string to a byte array.
            byte[] pdfBytes = Convert.FromBase64String(pdfData);

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

            // Attach the PDF
            Attachment attachment = new Attachment(new MemoryStream(pdfBytes), "Document.pdf");
            message.Attachments.Add(attachment);

            // Create a System.Net.Mail.SmtpClient object
            // and set the SMTP host and port number
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

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
            return RedirectToAction("Index");
        }



    }
}
