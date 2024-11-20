using System;
using System.Collections.Generic;

namespace ServiceWorkerWebsite.Models
{
    public class BusinessEarningsViewModel
    {
        public Worker Worker { get; set; }
        public List<Service> Services { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<ServiceEarningsViewModel> ServiceEarnings { get; set; }
        public List<MonthlyEarningsViewModel> MonthlyEarnings { get; set; }
        public decimal TotalEarnings { get; set; }
        public int UniqueCustomers { get; set; }
    }

    public class ServiceEarningsViewModel
    {
        public string ServiceName { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalEarnings { get; set; }
    }

    public class MonthlyEarningsViewModel
    {
        public DateTime Month { get; set; }
        public decimal TotalEarnings { get; set; }
        public int NumberOfBookings { get; set; }
    }
}