using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;
using MimeKit;

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

        public async Task<IActionResult> AddToShoppingCartBoxes(int pizzaId, int amount = 1)
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
       


        public async Task<IActionResult> GeneratePdf()

        {
            var shoppingCartItems = await _shoppingCart.GetShoppingCartItemsAsync();

            Document Doc = new Document(PageSize.LETTER);

            //Create our file stream
            using (FileStream fs = new FileStream(@"C:\Users\rivky\OneDrive\היה פה\שולחן העבודה\h.pdf", FileMode.Create, FileAccess.Write, FileShare.Read))
            {


                var currentDate = DateTime.Now;
                // Get the customer name and shipping ID from your data source
                var customerName = "Yaeli Gloiberman";
                var shippingId = "213245632";
                // Create a new PDF document

                // Set the response content type
                Response.ContentType = "application/pdf";
                // Set the file name for the PDF
                Response.Headers.Add("Content-Disposition", "attachment; filename=shopping_cart.pdf");
                // Create a new PDF writer
                PdfWriter writer = PdfWriter.GetInstance(Doc, Response.Body);
                // Set the page size to A4
                Doc.SetPageSize(PageSize.A4);
                // Set the margins for the document
                Doc.SetMargins(36f, 36f, 36f, 36f);
                // Open the PDF document
                Doc.Open();
                // Add the date, customer name, and shipping ID to the PDF document
                var paragraph = new Paragraph($"Date: {currentDate.ToShortDateString()}    Customer: {customerName}    Shipping ID: {shippingId}");
                paragraph.SpacingAfter = 20f;
                Doc.Add(paragraph);
                
                //Full path to the Arial file
                string ARIALUNI_TFF = System.IO.Path.Combine(@"C:\\Windows\\Fonts", "arial.ttf");

                //Create a base font object making sure to specify IDENTITY-H
                BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                //Create a specific font object
                iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 14);

                //Use a table so that we can set the text direction
                PdfPTable T = new PdfPTable(5);
                T.WidthPercentage = 90;
                T.DefaultCell.BorderWidth = 1;
                T.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                T.AddCell(new Phrase("שם מוצר", f));
                T.AddCell(new Phrase("ברקוד", f));
                T.AddCell(new Phrase("כמות בארגז", f));
                T.AddCell(new Phrase("כמות ביחידים", f));
                T.AddCell(new Phrase("מחיר", f));

                foreach (var item in shoppingCartItems)
                {
                    T.AddCell(new Phrase(item.Pizza.Name, f));
                    T.AddCell(new Phrase(item.Pizza.Barcode, f));
                    T.AddCell(new Phrase(item.AmountBoxes.ToString(), f));
                    T.AddCell(new Phrase(item.AmountSingles.ToString(), f));
                    T.AddCell(new Phrase(item.Pizza.Price.ToString(), f));
                }
                Doc.Add(T);

                if (writer.GetVerticalPosition(false) < Doc.BottomMargin)
                {
                    // If the table extends beyond the current page, add a new page
                    Doc.NewPage();
                    // Add the date, customer name, and shipping ID to the new page
                    Doc.Add(paragraph);
                    // Add the table to the new page
                    Doc.Add(T);
                }
                // Close the PDF document
                Doc.Close();
                return new EmptyResult();

            }

        }

        public async Task<IActionResult> GeneratePdf1(string name, string phone, string email, string notes)

        {
            var shoppingCartItems = await _shoppingCart.GetShoppingCartItemsAsync();

            Document Doc = new Document(PageSize.LETTER);

            //Create our file stream
            using (FileStream fs = new FileStream(@"C:\Users\rivky\OneDrive\היה פה\שולחן העבודה\h.pdf", FileMode.Create, FileAccess.Write, FileShare.Read))
            {


                var currentDate = DateTime.Now;
                // Get the customer name and shipping ID from your data source
                var customerName = name;
                var shippingId = phone;
                // Create a new PDF document

                // Set the response content type
                Response.ContentType = "application/pdf";
                // Set the file name for the PDF
                Response.Headers.Add("Content-Disposition", "attachment; filename=shopping_cart.pdf");
                // Create a new PDF writer
                PdfWriter writer = PdfWriter.GetInstance(Doc, Response.Body);
                // Set the page size to A4
                Doc.SetPageSize(PageSize.A4);
                // Set the margins for the document
                Doc.SetMargins(36f, 36f, 36f, 36f);
                // Open the PDF document
                Doc.Open();
                // Add the date, customer name, and shipping ID to the PDF document
                var paragraph = new Paragraph($"Date: {currentDate.ToShortDateString()}    Customer: {customerName}    Shipping ID: {shippingId}");
                paragraph.SpacingAfter = 20f;
                Doc.Add(paragraph);

                //Full path to the Arial file
                string ARIALUNI_TFF = System.IO.Path.Combine(@"C:\\Windows\\Fonts", "arial.ttf");

                //Create a base font object making sure to specify IDENTITY-H
                BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                //Create a specific font object
                iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 14);

                //Use a table so that we can set the text direction
                PdfPTable T = new PdfPTable(5);
                T.WidthPercentage = 90;
                T.DefaultCell.BorderWidth = 1;
                T.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                T.AddCell(new Phrase("שם מוצר", f));
                T.AddCell(new Phrase("ברקוד", f));
                T.AddCell(new Phrase("כמות בארגז", f));
                T.AddCell(new Phrase("כמות ביחידים", f));
                T.AddCell(new Phrase("מחיר", f));

                foreach (var item in shoppingCartItems)
                {
                    T.AddCell(new Phrase(item.Pizza.Name, f));
                    T.AddCell(new Phrase(item.Pizza.Barcode, f));
                    T.AddCell(new Phrase(item.AmountBoxes.ToString(), f));
                    T.AddCell(new Phrase(item.AmountSingles.ToString(), f));
                    T.AddCell(new Phrase(item.Pizza.Price.ToString(), f));
                }
                Doc.Add(T);

                if (writer.GetVerticalPosition(false) < Doc.BottomMargin)
                {
                    // If the table extends beyond the current page, add a new page
                    Doc.NewPage();
                    // Add the date, customer name, and shipping ID to the new page
                    Doc.Add(paragraph);
                    // Add the table to the new page
                    Doc.Add(T);
                }
                // Close the PDF document
                Doc.Close();
                return new EmptyResult();

            }

        }

        [HttpPost]
        public async Task<IActionResult> GenerateAndSendPdf()
        {
            var shoppingCartItems = await _shoppingCart.GetShoppingCartItemsAsync();

            Document Doc = new Document(PageSize.LETTER);

            //Create our file stream
            using (var memoryStream = new MemoryStream())
            {
                // Create a new PDF writer
                PdfWriter writer = PdfWriter.GetInstance(Doc, memoryStream);
                // Set the page size to A4
                Doc.SetPageSize(PageSize.A4);
                // Set the margins for the document
                Doc.SetMargins(36f, 36f, 36f, 36f);
                // Open the PDF document
                Doc.Open();

                var currentDate = DateTime.Now;
                // Get the customer name and shipping ID from your data source
                var customerName = "Yaeli Gloiberman";
                var shippingId = "213245632";
                // Add the date, customer name, and shipping ID to the PDF document
                var paragraph = new Paragraph($"Date: {currentDate.ToShortDateString()}    Customer: {customerName}    Shipping ID: {shippingId}");
                paragraph.SpacingAfter = 20f;
                Doc.Add(paragraph);

                //Full path to the Arial file
                string ARIALUNI_TFF = System.IO.Path.Combine(@"C:\\Windows\\Fonts", "arial.ttf");

                //Create a base font object making sure to specify IDENTITY-H
                BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                //Create a specific font object
                iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 14);

                //Use a table so that we can set the text direction
                PdfPTable T = new PdfPTable(5);
                T.WidthPercentage = 90;
                T.DefaultCell.BorderWidth = 1;
                T.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                T.AddCell(new Phrase("שם מוצר", f));
                T.AddCell(new Phrase("ברקוד", f));
                T.AddCell(new Phrase("כמות בארגז", f));
                T.AddCell(new Phrase("כמות ביחידים", f));
                T.AddCell(new Phrase("מחיר", f));

                foreach (var item in shoppingCartItems)
                {
                    T.AddCell(new Phrase(item.Pizza.Name, f));
                    T.AddCell(new Phrase(item.Pizza.Barcode, f));
                    T.AddCell(new Phrase(item.AmountBoxes.ToString(), f));
                    T.AddCell(new Phrase(item.AmountSingles.ToString(), f));
                    T.AddCell(new Phrase(item.Pizza.Price.ToString(), f));
                }
                Doc.Add(T);

                if (writer.GetVerticalPosition(false) < Doc.BottomMargin)
                {
                    // If the table extends beyond the current page, add a new page
                    Doc.NewPage();
                    // Add the date, customer name, and shipping ID to the new page
                    Doc.Add(paragraph);
                    // Add the table to the new page
                    Doc.Add(T);
                }
                // Close the PDF document
                Doc.Close();


                var pdfBytes = memoryStream.ToArray();

                // Create a System.Net.Mail.MailMessage object
                MailMessage message = new MailMessage();
                //message.From = new MailAddress("rvider@g.jct.ac.il");
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
                message.From = new MailAddress("rvider@g.jct.ac.il", "Kedma");

                // Create a MemoryStream object from your PDF file
                MemoryStream memoryStream1 = new MemoryStream(pdfBytes);

                // Create a new attachment with the PDF bytes
                Attachment attachment = new Attachment(memoryStream1, "shopping_cart.pdf", "application/pdf");

                // Add the attachment to the email message
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
}



