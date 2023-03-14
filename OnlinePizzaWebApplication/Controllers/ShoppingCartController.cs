using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlinePizzaWebApplication.Repositories;
using OnlinePizzaWebApplication.Models;
using OnlinePizzaWebApplication.ViewModels;
using OnlinePizzaWebApplication.Data;

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

        public async Task<IActionResult> AddToShoppingCart(int pizzaId, int amount=1)
        {
            var selectedPizza = await _pizzaRepository.GetByIdAsync(pizzaId);

            if (selectedPizza != null)
            {
                await _shoppingCart.AddToCartAsync(selectedPizza, amount, amount);
            }
            return RedirectToAction("Index", "Home");
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
      
      
        public async Task<IActionResult> SendToPdf(string emailAddress)
        {
            // Retrieve the items in the shopping cart
            List<CartItem> cartItems = await _shoppingCart.GetShoppingCartItemsAsync();

            // Create a new PDF document
            var document = new Document();
            var writer = PdfWriter.GetInstance(document, new MemoryStream());
            document.Open();

            // Add the shopping cart items to the PDF
            foreach (var item in cartItems)
            {
                document.Add(new Paragraph(item.Name));
                document.Add(new Paragraph(item.Barcode));
                // Add more details as needed
            }

            // Close the PDF document
            document.Close();

            // Create a new email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", "your-email@example.com"));
            message.To.Add(new MailboxAddress("", emailAddress));
            message.Subject = "Shopping Cart PDF";

            // Create a new MIME part for the PDF attachment
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(new MemoryStream(writer.DirectContentUnder.RawContent), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                FileName = "shopping_cart.pdf"
            };

            // Add the PDF attachment to the email message
            var multipart = new Multipart("mixed");
            multipart.Add(attachment);
            message.Body = multipart;

            // Connect to the SMTP server and send the email
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("your-email@example.com", "your-email-password");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            // Return a success message to the user
            return Content("PDF sent successfully!");
        }
    }
}