using System.Collections.Generic;

namespace ServiceWorkerWebsite.Models
{
    public class WorkerEarningsViewModel
    {
        public int WorkerId { get; set; }
        public decimal TotalEarnings { get; set; } // Total earnings of the worker
        public int TotalBookings { get; set; } // Total number of bookings
        public int ServicesCount { get; set; } // Total number of services offered
        public List<ServiceEarningsViewModel> ServiceEarnings { get; set; } // Earnings by service
    }

    public class ServiceEarningsViewModel
    {
        public string ServiceName { get; set; } // Name of the service
        public decimal BasePrice { get; set; } // Base price of the service
        public int TotalBookings { get; set; } // Number of bookings for the service
        public decimal TotalEarnings { get; set; } // Total earnings from the service
    }
}
