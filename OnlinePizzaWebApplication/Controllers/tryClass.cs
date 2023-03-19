//using iTextSharp.text.pdf;
//using iTextSharp.text;
//using MailKit.Security;
//using Microsoft.AspNetCore.Mvc;
//using MimeKit;
//using System.IO;
//using System.Net.Mail;
//using System.Net;
//using System.Threading.Tasks;
//using System;
//using OnlinePizzaWebApplication.Data;
//using OnlinePizzaWebApplication.Repositories;
//using OnlinePizzaWebApplication.Migrations;
//using System.Collections.Generic;
//using System.Linq;
//using OnlinePizzaWebApplication.Models;
//using OnlinePizzaWebApplication.ViewModels;
//using Microsoft.AspNetCore.Http;
//using Org.BouncyCastle.Asn1;
//using System.Collections.Specialized;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using iTextSharp.text.pdf.parser;
//using Microsoft.Extensions.Primitives;
//using System.Reflection;
//using Microsoft.CodeAnalysis;
//using System.Text.RegularExpressions;

//namespace OnlineKedmaWebApplication.Controllers
//{
//    public class tryClass
//    {
//        private readonly IPizzaRepository _pizzaRepository;
//        private readonly AppDbContext _context;
//        private readonly OnlinePizzaWebApplication.Migrations.ShoppingCart _shoppingCart;
//        public async Task<IActionResult> GenerateAndSendPdf()
//        {
//            var shoppingCartItems = await _shoppingCart.GetShoppingCartItemsAsync();

//            Document Doc = new Document(PageSize.LETTER);

//            //Create our file stream
//            using (var memoryStream = new MemoryStream())
//            {
//                // Create a new PDF writer
//                PdfWriter writer = PdfWriter.GetInstance(Doc, memoryStream);
//                // Set the page size to A4
//                Doc.SetPageSize(PageSize.A4);
//                // Set the margins for the document
//                Doc.SetMargins(36f, 36f, 36f, 36f);
//                // Open the PDF document
//                Doc.Open();

//                var currentDate = DateTime.Now;
//                // Get the customer name and shipping ID from your data source
//                var customerName = "Yaeli Gloiberman";
//                var shippingId = "213245632";
//                // Add the date, customer name, and shipping ID to the PDF document
//                var paragraph = new Paragraph($"Date: {currentDate.ToShortDateString()}    Customer: {customerName}    Shipping ID: {shippingId}");
//                paragraph.SpacingAfter = 20f;
//                Doc.Add(paragraph);

//                //Full path to the Arial file
//                string ARIALUNI_TFF = System.IO.Path.Combine(@"C:\\Windows\\Fonts", "arial.ttf");

//                //Create a base font object making sure to specify IDENTITY-H
//                BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

//                //Create a specific font object
//                iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 14);

//                //Use a table so that we can set the text direction
//                PdfPTable T = new PdfPTable(5);
//                T.WidthPercentage = 90;
//                T.DefaultCell.BorderWidth = 1;
//                T.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
//                T.AddCell(new Phrase("שם מוצר", f));
//                T.AddCell(new Phrase("ברקוד", f));
//                T.AddCell(new Phrase("כמות בארגז", f));
//                T.AddCell(new Phrase("כמות ביחידים", f));
//                T.AddCell(new Phrase("מחיר", f));

//                foreach (var item in shoppingCartItems)
//                {
//                    T.AddCell(new Phrase(item.Pizza.Name, f));
//                    T.AddCell(new Phrase(item.Pizza.Barcode, f));
//                    T.AddCell(new Phrase(item.AmountBoxes.ToString(), f));
//                    T.AddCell(new Phrase(item.AmountSingles.ToString(), f));
//                    T.AddCell(new Phrase(item.Pizza.Price.ToString(), f));
//                }
//                Doc.Add(T);

//                if (writer.GetVerticalPosition(false) < Doc.BottomMargin)
//                {
//                    // If the table extends beyond the current page, add a new page
//                    Doc.NewPage();
//                    // Add the date, customer name, and shipping ID to the new page
//                    Doc.Add(paragraph);
//                    // Add the table to the new page
//                    Doc.Add(T);
//                }
//                // Close the PDF document
//                Doc.Close();

//                var pdfBytes = memoryStream.ToArray();

//                // Create a System.Net.Mail.MailMessage object
//                MailMessage message = new MailMessage();

//                // Add a recipient
//                message.To.Add("rvider@g.jct.ac.il");

//                // Add a message subject
//                message.Subject = "Account activation";

//                // Add a message body
//                message.Body = "Hello customer" + "\n" +
//                    "Welcome to Kedma.\n" +
//                    "We are so happy you have chosen to join us  and  are sure you will enjoy our services.\n" +
//                    "For any question, problems or requests you can contact our customer service team " +
//                    "at customerService@DroneDrop.com and they will gladly assist you.\n\n" +
//                    "Thank you and have a great day,\n" +
//                    "Team DroneDrop";

//                // Create a System.Net.Mail.MailAddress object and 
//                // set the sender email address and display name.
//                message.From = new MailAddress("rvider@g.jct.ac.il", "Kedma");

//                // Create a new attachment with the PDF bytes
//                var attachment = new MimePart("application", "pdf")
//                {
//                    Content = new MimeContent(memoryStream),
//                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
//                    ContentTransferEncoding = ContentEncoding.Base64,
//                    FileName = "shopping_cart.pdf"
//                };

//                // Add the attachment to the email message
//                var multipart = new Multipart("mixed");
//                multipart.Add(attachment);
//                message.Body = multipart;

//                // Create a System.Net.Mail.SmtpClient object
//                // and set the SMTP host and port number
//                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

//                // If your server requires authentication add the below code
//                // =========================================================
//                // Enable Secure Socket Layer (SSL) for connection encryption
//                smtp.EnableSsl = true;

//                // Do not send the DefaultCredentials with requests
//                smtp.UseDefaultCredentials = false;

//                // Create a System.Net.NetworkCredential object and set
//                // the username and password required by your SMTP account
//                smtp.Credentials = new NetworkCredential("dronedrop2021@gmail.com", "ouiatjczzhkvtxnr");
//                // =========================================================

//                // Send the message
//                smtp.Send(message);


//                // Redirect to the previous page
//                return RedirectToAction("Index");
//            }
//        }


//        [HttpPost]
//        public IActionResult SendEmail()
//        {
//            // Create a System.Net.Mail.MailMessage object
//            MailMessage message = new MailMessage();

//            // Add a recipient
//            message.To.Add("rvider@g.jct.ac.il");

//            // Add a message subject
//            message.Subject = "Account activation";

//            // Add a message body
//            message.Body = "Hello customer" + "\n" +
//                "Welcome to Kedma.\n" +
//                "We are so happy you have chosen to join us  and  are sure you will enjoy our services.\n" +
//                "For any question, problems or requests you can contact our customer service team " +
//                "at customerService@DroneDrop.com and they will gladly assist you.\n\n" +
//                "Thank you and have a great day,\n" +
//                "Team DroneDrop";

//            // Create a System.Net.Mail.MailAddress object and 
//            // set the sender email address and display name.
//            message.From = new MailAddress("rvider@g.jct.ac.il", "Kedma");

//            // Create a System.Net.Mail.SmtpClient object
//            // and set the SMTP host and port number
//            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

//            // If your server requires authentication add the below code
//            // =========================================================
//            // Enable Secure Socket Layer (SSL) for connection encryption
//            smtp.EnableSsl = true;

//            // Do not send the DefaultCredentials with requests
//            smtp.UseDefaultCredentials = false;

//            // Create a System.Net.NetworkCredential object and set
//            // the username and password required by your SMTP account
//            smtp.Credentials = new NetworkCredential("dronedrop2021@gmail.com", "ouiatjczzhkvtxnr");
//            // =========================================================

//            // Send the message
//            smtp.Send(message);

//            // Redirect to the previous page
//            return RedirectToAction("Index");
//        }
//    }
//}
