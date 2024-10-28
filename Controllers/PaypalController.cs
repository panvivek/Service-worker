using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaypalCheckoutExample.Clients;
using ServiceWorkerWebsite.Data;

namespace PaypalCheckoutExample.Controllers
{
    public class PaypalController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly ApplicationDbContext _context;


        public PaypalController(PaypalClient paypalClient, ApplicationDbContext context)
        {
            this._paypalClient = paypalClient;
            this._context = context;
        }

        public IActionResult Index(int workerId)
        {
            ViewBag.WorkerId = workerId;
            ViewBag.ClientId = _paypalClient.ClientId;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Order([FromQuery] int workerId, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch the worker's price from the database using workerId
                var worker = await _context.Worker_List.FindAsync(workerId);
                if (worker == null)
                {
                    return BadRequest(new { message = "Worker not found" });
                }

                // Use the worker's price for the transaction
                var price = worker.Price.ToString("F2");  // Format to 2 decimal places
                var currency = "USD";
                var reference = $"INV{workerId:D6}";  // Use a unique invoice reference

                // Create the PayPal order
                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new { e.GetBaseException().Message };
                return BadRequest(error);
            }
        }


        public async Task<IActionResult> Capture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = response.purchase_units[0].reference_id;

                // Put your logic to save the transaction here
                // You can use the "reference" variable as a transaction key

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}