using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf; // Don't forget to add these namespaces
using System.IO;

namespace ServiceWorkerWebsite.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;



        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Include related entities for Worker, Service, and TimeSlot
            var bookings = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.Worker)
                .Include(b => b.TimeSlot)  // Include TimeSlot to display appointment details
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View(bookings);
        }




        // GET: Bookings/Create

        public IActionResult Create(int workerId, int serviceId)
        {
            ViewData["Worker_Id"] = workerId;
            ViewData["Service_Id"] = serviceId;
            Console.WriteLine($"Worker ID........................................................: {workerId}");
            Console.WriteLine($"Service ID: {serviceId}");

            return View();
        }
        // POST: Bookings/GetAvailableSlots
        [HttpPost]
        public async Task<JsonResult> GetAvailableSlots([FromBody] WorkerRequest request)
        {
            var workerId = request.WorkerId; // Accessing the workerId from the request object
            Console.WriteLine($"Entered GetAvailableSlots Method");
            Console.WriteLine($"Worker ID in GetAvailableSlots: {workerId}");

            // Fetch available slots and sort them
            var availableSlots = await _context.TimeSlot_List
                .Where(ts => ts.Worker_Id == workerId && !ts.IsBooked)
                .GroupBy(ts => ts.SelectedDates)
                .Select(g => new
                {
                    date = g.Key,
                    timeSlots = g.OrderBy(ts => ts.TimeSlots).Select(ts => new
                    {
                        TimeSlotId = ts.TimeSlotId,
                        TimeSlots = ts.TimeSlots
                    }).ToList()
                })
                .OrderBy(group => group.date) // Sort groups by date
                .ToListAsync();

            // Debugging: Print available slots to the console
            if (availableSlots.Count == 0)
            {
                Console.WriteLine("No available slots found.");
            }
            else
            {
                foreach (var group in availableSlots)
                {
                    Console.WriteLine($"Date: {group.date}");
                    foreach (var slot in group.timeSlots)
                    {
                        Console.WriteLine($"TimeSlotId: {slot.TimeSlotId}, TimeSlots: {slot.TimeSlots}");
                    }
                }
            }

            return Json(availableSlots);
        }




        // Define a class to deserialize the request
        public class WorkerRequest
        {
            public int WorkerId { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Worker_Id,BookingDate,Service_Id,AgreeToTerms,TimeSlotId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Get the current logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // This gets the logged-in user's ID

                // Assign the UserId to the booking
                booking.UserId = userId;

                // Find the time slot in the database using the provided TimeSlotId
                var timeSlot = await _context.TimeSlot_List.FindAsync(booking.TimeSlotId);

                if (timeSlot != null)
                {
                    timeSlot.IsBooked = true;
                    _context.Update(timeSlot); // Update the time slot in the context
                }
                else
                {
                    ModelState.AddModelError("TimeSlotId", "Selected time slot is not valid.");
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }




        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", booking.Service_Id);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Service_Id,Worker_Id,BookingDate,,AgreeToTerms,TimeSlotId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", booking.Service_Id);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.Worker)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }

        //---------------------------------------------------------------------------------------------------
        //--------------------------------INVOICE GENERATOR--------------------------------------------------
        //---------------------------------------------------------------------------------------------------

        public async Task<IActionResult> DownloadInvoice(int id)
        {
            // Get the booking details from the database
            var booking = await _context.Booking
                .Include(b => b.Worker)
                    .ThenInclude(w => w.User) // Ensure to include the User entity
                .Include(b => b.Service)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Generate a random invoice number
            var random = new Random();
            var invoiceNumber = $"INV-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            // Generate PDF
            using (var memoryStream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A5);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                // Create Fonts
                var titleFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD);
                var boldFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
                var normalFont = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL);

                // Add Company Logo
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Logo_QuickFix_Black.png");
                if (System.IO.File.Exists(logoPath))
                {
                    Image logo = Image.GetInstance(logoPath);
                    logo.ScaleAbsolute(120f, 60f); // Adjust the size as necessary
                    logo.Alignment = Element.ALIGN_LEFT;
                    pdfDoc.Add(logo);
                }

                // Add a table to structure the header information
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 70, 30 }); // Set column widths (70% for company details, 30% for invoice details)

                // Company details cell with bold title
                PdfPCell companyDetailsCell = new PdfPCell();
                companyDetailsCell.Border = PdfPCell.NO_BORDER;
                companyDetailsCell.VerticalAlignment = Element.ALIGN_TOP;
                companyDetailsCell.AddElement(new Paragraph("QuickFix", boldFont));
                companyDetailsCell.AddElement(new Paragraph("7899 McLaughlin Rd\nBrampton, ON, L6Y 0P8\nCanada", normalFont));
                headerTable.AddCell(companyDetailsCell);

                // Invoice details cell (aligned to the right)
                PdfPCell invoiceDetailsCell = new PdfPCell();
                invoiceDetailsCell.Border = PdfPCell.NO_BORDER;
                invoiceDetailsCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                invoiceDetailsCell.VerticalAlignment = Element.ALIGN_TOP;
                invoiceDetailsCell.AddElement(new Paragraph($"Invoice No: {invoiceNumber}", boldFont));
                invoiceDetailsCell.AddElement(new Paragraph($"Date: {booking.BookingDate.ToString("dd-MM-yyyy")}", boldFont));
                headerTable.AddCell(invoiceDetailsCell);

                // Add the table to the document
                pdfDoc.Add(headerTable);
                pdfDoc.Add(new Paragraph("\n")); // Add some space below the header
                pdfDoc.Add(new Paragraph("\n")); // Add some space below the header


                // Worker and Booking Information
                var workerName = booking.Worker != null && booking.Worker.User != null
                    ? $"{booking.Worker.User.Firstname} {booking.Worker.User.Lastname}"
                    : "No Worker Assigned";

                // Add worker information with labels in bold
                Paragraph workerParagraph = new Paragraph();
                workerParagraph.Add(new Chunk("Worker: ", boldFont));
                workerParagraph.Add(new Chunk(workerName, normalFont));
                pdfDoc.Add(workerParagraph);

                // Add booking date information with label in bold
                Paragraph bookingDateParagraph = new Paragraph();

                pdfDoc.Add(new Paragraph("\n")); // Blank line for spacing
                bookingDateParagraph.Add(new Chunk("Booking Date: ", boldFont));
                bookingDateParagraph.Add(new Chunk(booking.BookingDate.ToString("dd-MM-yyyy"), normalFont));
                pdfDoc.Add(bookingDateParagraph);

                // Add time slot information with label in bold
                Paragraph timeSlotParagraph = new Paragraph();

                pdfDoc.Add(new Paragraph("\n")); // Blank line for spacing
                timeSlotParagraph.Add(new Chunk("Time Slot: ", boldFont));
                if (booking.TimeSlot != null)
                {
                    timeSlotParagraph.Add(new Chunk($"{booking.TimeSlot.SelectedDates} {booking.TimeSlot.TimeSlots}", normalFont));
                }
                else
                {
                    timeSlotParagraph.Add(new Chunk("Not Assigned", normalFont));
                }
                pdfDoc.Add(timeSlotParagraph);

                // Add service information with label in bold
                Paragraph serviceParagraph = new Paragraph();
                
                pdfDoc.Add(new Paragraph("\n")); // Blank line for spacing
                serviceParagraph.Add(new Chunk("Service: ", boldFont));
              //  serviceParagraph.Add(new Chunk($"{booking.Service?.Service_Id ?? "Not Assigned"}", normalFont));
                pdfDoc.Add(serviceParagraph);

                pdfDoc.Add(new Paragraph("\n")); // Blank line for spacing
                pdfDoc.Add(new Paragraph("Payment Amount:", boldFont));


                pdfDoc.Add(new Paragraph("\n")); // Blank line for spacing

                // Thank You Note and Footer
                pdfDoc.Add(new Paragraph("\n\n\n\n\n\n\n\n")); // Blank line for spacing
                pdfDoc.Add(new Paragraph("Thank you for your business!", boldFont));
                pdfDoc.Add(new Paragraph("For inquiries, contact us at: support@quickfix.com", normalFont));

                pdfDoc.Close();

                // Return the PDF as a file download
                return File(memoryStream.ToArray(), "application/pdf", "Invoice.pdf");
            }
        }




    }

    public class DateTimeRequest
    {
        public DateTime Date { get; set; }
        public int WorkerId { get; set; }
    }
}