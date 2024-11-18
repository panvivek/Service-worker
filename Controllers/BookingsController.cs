using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceWorkerWebsite.Areas.Identity.Data;
using ServiceWorkerWebsite.Data;
using ServiceWorkerWebsite.Models;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf; // Don't forget to add these namespaces
using System.IO;

namespace ServiceWorkerWebsite.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ServiceWorkerWebsiteUser> _userManager;


        public BookingsController(ApplicationDbContext context, UserManager<ServiceWorkerWebsiteUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Include related entities for Worker, Service, and TimeSlot
            var bookings = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.Worker) // Include Worker to access UserId
                .ThenInclude(w => w.User) // Include User to access first and last name
                .Include(b => b.TimeSlot) // Include TimeSlot to display appointment details
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View(bookings);
        }




        // GET: Bookings/Create

        public IActionResult Create(int workerId, int serviceId)
        {
            // Log the serviceId and workerId values
            Console.WriteLine($"Worker ID: {workerId}");
            Console.WriteLine($"Service ID: {serviceId}");

            ViewData["Worker_Id"] = workerId;
            ViewData["Service_Id"] = serviceId;

            return View();
        

    }


    [HttpPost]
public async Task<JsonResult> GetAvailableSlots([FromBody] WorkerRequest request)
{
    var workerId = request.WorkerId; // Accessing the workerId from the request object
    var serviceId = request.ServiceId; // Accessing the serviceId from the request object
    Console.WriteLine($"Entered GetAvailableSlots Method");
    Console.WriteLine($"Worker ID: {workerId}");
    Console.WriteLine($"Service ID: {serviceId}"); // Log the serviceId

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



        // Fetching current user booked time slots and all avialble timeslots 

        [HttpPost]
        public async Task<JsonResult> EditAvailableSlots([FromBody] WorkerRequest request)
        {
            var workerId = request.WorkerId; // Accessing the workerId from the request object
            var serviceId = request.ServiceId; // Accessing the serviceId from the request object
            var timeSlotId = request.TimeSlotId; // Accessing the timeSlotId from the request object
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user ID
            Console.WriteLine($"Entered EditAvailableSlots Method");
            Console.WriteLine($"Worker ID: {workerId}");
            Console.WriteLine($"Service ID: {serviceId}"); // Log the serviceId
            Console.WriteLine($"timeslot ID: {timeSlotId}"); // Log the serviceId

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

            // Fetch the current booking for the logged-in user using TimeSlotId
            var currentBooking = await _context.Booking
                .Where(b => b.UserId == currentUserId && b.Worker_Id == workerId && b.Service_Id == serviceId && b.TimeSlotId == timeSlotId)
                .Select(b => new
                {
                    // Change BookingDate to the selected date from TimeSlot_List
                    Date = _context.TimeSlot_List
                        .Where(ts => ts.TimeSlotId == b.TimeSlotId)
                        .Select(ts => ts.SelectedDates)
                        .FirstOrDefault(), // Assuming SelectedDates is the property name
                    TimeSlotId = b.TimeSlotId
                })
                .FirstOrDefaultAsync();

            // Fetch the time slot details for the current booking if it exists
            string currentTimeSlot = null;
            if (currentBooking != null)
            {
                currentTimeSlot = await _context.TimeSlot_List
                    .Where(ts => ts.TimeSlotId == currentBooking.TimeSlotId)
                    .Select(ts => ts.TimeSlots)
                    .FirstOrDefaultAsync();
            }

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

            // Return both available slots and current booking time slot (if any)
            return Json(new
            {
                AvailableSlots = availableSlots,
                CurrentBooking = currentBooking != null ? new
                {
                    Date = currentBooking.Date,
                    TimeSlotId = currentBooking.TimeSlotId,
                    TimeSlots = currentTimeSlot // Include the time slot details
                } : null
            });
        }




        // Define a class to deserialize the request
        public class WorkerRequest
        {
            public int WorkerId { get; set; }
            public int ServiceId { get; set; }
            public int TimeSlotId { get; set; }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Worker_Id,BookingDate,Service_Id,AgreeToTerms,TimeSlotId")] Booking booking)
        {
            // Print all relevant data to the console
            Console.WriteLine($"Worker ID: {booking.Worker_Id}");
            Console.WriteLine($"Service ID: {booking.Service_Id}");
            Console.WriteLine($"Booking Date: {booking.BookingDate}");
            Console.WriteLine($"Agree to Terms: {booking.AgreeToTerms}");
            Console.WriteLine($"Time Slot ID: {booking.TimeSlotId}");

            // Get the current logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // This gets the logged-in user's ID
            Console.WriteLine($"User ID: {userId}");

            // Assign the UserId to the booking
            booking.UserId = userId;

            if (ModelState.IsValid)
            {
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

                return RedirectToAction("Index", "Paypal", new { workerId = booking.Worker_Id });

            }

            return View(booking);
        }




        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var currentUser = await _userManager.GetUserAsync(User);

            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            // Set the TimeSlotId from the booking entity
            ViewData["TimeSlotId"] = booking.TimeSlotId; // Assuming TimeSlotId is a property of Booking
            ViewData["Service_Id"] = new SelectList(_context.Services_List, "Service_Id", "Service_Id", booking.Service_Id);

            return View(booking);
        }

        // POST: Bookings/Edit/5
        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Service_Id,Worker_Id,BookingDate,AgreeToTerms")] Booking booking, int TimeSlotId)
        {
            // Log the incoming TimeSlotId for debugging
            Console.WriteLine($"Editing Booking ID: {id}");
            Console.WriteLine($"Incoming TimeSlotId: {TimeSlotId}");

            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the previous booking
                    var previousBooking = await _context.Booking.FindAsync(id);
                    if (previousBooking != null)
                    {
                        // Log previous booking details for debugging
                        Console.WriteLine($"Previous Booking Details - Worker ID: {previousBooking.Worker_Id}, TimeSlotId: {previousBooking.TimeSlotId}");

                        // Mark the previously booked slot as not booked
                        var previousTimeSlot = await _context.TimeSlot_List.FindAsync(previousBooking.TimeSlotId);
                        if (previousTimeSlot != null)
                        {
                            previousTimeSlot.IsBooked = false; // Unmark the old time slot
                            Console.WriteLine($"Unmarking Previous TimeSlotId: {previousBooking.TimeSlotId}");
                        }

                        // Check if the new TimeSlotId exists
                        var newTimeSlot = await _context.TimeSlot_List.FindAsync(TimeSlotId);
                        if (newTimeSlot != null)
                        {
                            newTimeSlot.IsBooked = true; // Mark the new time slot as booked
                            Console.WriteLine($"Marking New TimeSlotId: {TimeSlotId} as booked");
                        }
                        else
                        {
                            ModelState.AddModelError("TimeSlotId", "The selected time slot does not exist.");
                            return View(booking);
                        }

                        // Update the booking information
                        previousBooking.Service_Id = booking.Service_Id;
                        previousBooking.Worker_Id = booking.Worker_Id;
                        previousBooking.BookingDate = booking.BookingDate;
                        previousBooking.AgreeToTerms = booking.AgreeToTerms;
                        previousBooking.TimeSlotId = TimeSlotId; // Update the TimeSlotId for the booking

                        // Save changes to the context
                        _context.Update(previousBooking);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
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
            }

            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the current logged-in worker's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // First get the worker record for the logged-in user
            var worker = await _context.Worker_List
                .FirstOrDefaultAsync(w => w.UserId == currentUserId);

            if (worker == null)
            {
                return NotFound("Worker not found");
            }

            // Find the booking using the provided id and include all necessary related data
            var booking = await _context.Booking
                .Include(b => b.Service)
                .Include(b => b.TimeSlot)
                .Include(b => b.User)  // Include the customer details
                .FirstOrDefaultAsync(m => m.Id == id && m.Worker_Id == worker.Worker_Id);

            if (booking == null)
            {
                return NotFound("Booking not found");
            }

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
                    .ThenInclude(w => w.User)
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null || booking.TimeSlot == null)
            {
                return NotFound();
            }

            try
            {
                // Parse the selected date from TimeSlot (format: yyyy-MM-dd)
                if (DateTime.TryParseExact(booking.TimeSlot.SelectedDates, "yyyy-MM-dd", null,
                    System.Globalization.DateTimeStyles.None, out DateTime selectedDate))
                {
                    // Parse time from TimeSlots (format: "10:30-11:15 PM")
                    var startTime = booking.TimeSlot.TimeSlots.Split('-')[0].Trim();

                    // Combine date and time
                    if (DateTime.TryParse($"{selectedDate.ToString("yyyy-MM-dd")} {startTime}", out DateTime appointmentDateTime))
                    {
                        var is24HoursBefore = appointmentDateTime > DateTime.Now.AddHours(24);

                        ViewData["CanCancel"] = is24HoursBefore;
                        ViewData["AppointmentDateTime"] = selectedDate;

                        // Debug logging
                        Console.WriteLine($"Selected Date: {selectedDate}");
                        Console.WriteLine($"Time Slot: {startTime}");
                        Console.WriteLine($"Appointment Time: {appointmentDateTime}");
                        Console.WriteLine($"Current Time: {DateTime.Now}");
                        Console.WriteLine($"24h from now: {DateTime.Now.AddHours(24)}");
                        Console.WriteLine($"Can Cancel: {is24HoursBefore}");
                    }
                    else
                    {
                        ViewData["CanCancel"] = false;
                        ViewData["AppointmentDateTime"] = selectedDate;
                    }
                }
                else
                {
                    ViewData["CanCancel"] = false;
                    ViewData["AppointmentDateTime"] = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in date parsing: {ex.Message}");
                ViewData["CanCancel"] = false;
                ViewData["AppointmentDateTime"] = DateTime.Now;
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null || booking.TimeSlot == null)
            {
                return NotFound();
            }

            try
            {
                // Parse the selected date from TimeSlot
                if (!DateTime.TryParseExact(booking.TimeSlot.SelectedDates, "yyyy-MM-dd", null,
                    System.Globalization.DateTimeStyles.None, out DateTime selectedDate))
                {
                    TempData["ErrorMessage"] = "Invalid appointment date format.";
                    return RedirectToAction(nameof(Index));
                }

                // Parse time from TimeSlots
                var startTime = booking.TimeSlot.TimeSlots.Split('-')[0].Trim();
                if (!DateTime.TryParse($"{selectedDate.ToString("yyyy-MM-dd")} {startTime}", out DateTime appointmentDateTime))
                {
                    TempData["ErrorMessage"] = "Invalid appointment time format.";
                    return RedirectToAction(nameof(Index));
                }

                var is24HoursBefore = appointmentDateTime > DateTime.Now.AddHours(24);

                if (!is24HoursBefore)
                {
                    TempData["ErrorMessage"] = "Cannot cancel appointments within 24 hours of the scheduled time.";
                    return RedirectToAction(nameof(Index));
                }

                // Free up the time slot
                var timeSlotToUpdate = await _context.TimeSlot_List.FindAsync(booking.TimeSlotId);
                if (timeSlotToUpdate != null)
                {
                    timeSlotToUpdate.IsBooked = false;
                    _context.TimeSlot_List.Update(timeSlotToUpdate);
                }

                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Booking cancelled successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in delete confirmation: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }


        public IActionResult TermsNCond()
        {
            return View();
        }


        public async Task<IActionResult> DownloadInvoice(int id)
        {
            // Fetch booking details from the database
            var booking = await _context.Booking
            .Include(b => b.Worker)
              .ThenInclude(w => w.User)
            .Include(b => b.Service)
            .Include(b => b.TimeSlot)
            .FirstOrDefaultAsync(b => b.Id == id);

            int? workerPrice = await _context.Worker_List
              .Where(w => w.Worker_Id == booking.Worker.Worker_Id)
              .Select(w => (int?)w.Price)
              .FirstOrDefaultAsync();

            if (booking == null)
            {
                return NotFound();
            }

            // Generate random invoice number
            var invoiceNumber = $"INV-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            using (var memoryStream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A5, 20, 20, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                // Fonts
                var titleFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD);
                var boldFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
                var normalFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);
                var headerFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE);

                // Add Logo
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Logo_Black.png");
                if (System.IO.File.Exists(logoPath))
                {
                    Image logo = Image.GetInstance(logoPath);
                    logo.ScaleAbsolute(120f, 60f);
                    pdfDoc.Add(logo);
                }

                // Header Table
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 70, 30 });

                // Company Info
                PdfPCell companyCell = new PdfPCell();
                companyCell.Border = PdfPCell.NO_BORDER;
                companyCell.AddElement(new Paragraph("QuickFix", titleFont));
                companyCell.AddElement(new Paragraph("7899 McLaughlin Rd\nBrampton, ON, L6Y 0P8\nCanada", normalFont));
                headerTable.AddCell(companyCell);

                // Invoice Info
                PdfPCell invoiceCell = new PdfPCell();
                invoiceCell.Border = PdfPCell.NO_BORDER;
                invoiceCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                invoiceCell.AddElement(new Paragraph($"Invoice No: {invoiceNumber}", boldFont));
                invoiceCell.AddElement(new Paragraph($"Date: {DateTime.Now.ToString("dd-MM-yyyy")}", normalFont));
                headerTable.AddCell(invoiceCell);

                pdfDoc.Add(headerTable);
                pdfDoc.Add(new Paragraph("\n"));

                // Booking Details Table
                PdfPTable detailsTable = new PdfPTable(2);
                detailsTable.WidthPercentage = 100;
                detailsTable.SpacingBefore = 10;
                detailsTable.SpacingAfter = 10;
                detailsTable.SetWidths(new float[] { 30, 70 });

                PdfPCell headerCell = new PdfPCell(new Phrase("Booking Details", headerFont))
                {
                    Colspan = 2,
                    BackgroundColor = BaseColor.DARK_GRAY,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                detailsTable.AddCell(headerCell);

                detailsTable.AddCell(new PdfPCell(new Phrase("Worker:", boldFont)));
                detailsTable.AddCell(new PdfPCell(new Phrase(
                  booking.Worker != null ? $"{booking.Worker.User.Firstname} {booking.Worker.User.Lastname}" : "No Worker Assigned", normalFont)));

                detailsTable.AddCell(new PdfPCell(new Phrase("Service:", boldFont)));
                detailsTable.AddCell(new PdfPCell(new Phrase(
                  booking.Service != null ? booking.Service.Name : "Not Assigned", normalFont)));

                detailsTable.AddCell(new PdfPCell(new Phrase("Booking Date:", boldFont)));
                detailsTable.AddCell(new PdfPCell(new Phrase(booking.BookingDate.ToString("dd-MM-yyyy"), normalFont)));

                detailsTable.AddCell(new PdfPCell(new Phrase("Time Slot:", boldFont)));
                detailsTable.AddCell(new PdfPCell(new Phrase(
                  booking.TimeSlot != null ? $"{booking.TimeSlot.SelectedDates} {booking.TimeSlot.TimeSlots}" : "Not Assigned", normalFont)));

                pdfDoc.Add(detailsTable);

                // Payment Section
                PdfPTable paymentTable = new PdfPTable(2);
                paymentTable.WidthPercentage = 100;
                paymentTable.SpacingBefore = 10;
                paymentTable.SetWidths(new float[] { 70, 30 });

                PdfPCell paymentHeader = new PdfPCell(new Phrase("Payment Details", headerFont))
                {
                    Colspan = 2,
                    BackgroundColor = BaseColor.DARK_GRAY,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                paymentTable.AddCell(paymentHeader);

                paymentTable.AddCell(new PdfPCell(new Phrase("Amount:", boldFont)));
                PdfPCell amountCell = new PdfPCell(new Phrase(workerPrice.HasValue ? workerPrice.Value.ToString("C") : "Not Assigned", boldFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                paymentTable.AddCell(amountCell);

                pdfDoc.Add(paymentTable);

                // Footer
                pdfDoc.Add(new Paragraph("\nThank you for your business!", boldFont) { Alignment = Element.ALIGN_CENTER });
                pdfDoc.Add(new Paragraph("For inquiries, contact support@quickfix.com", normalFont) { Alignment = Element.ALIGN_CENTER });

                pdfDoc.Close();
                return File(memoryStream.ToArray(), "application/pdf", $"Invoice_{invoiceNumber}.pdf");
            }
        }


    }

    public class DateTimeRequest
    {
        public DateTime Date { get; set; }
        public int WorkerId { get; set; }
    }
}