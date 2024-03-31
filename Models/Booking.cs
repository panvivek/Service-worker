using System.ComponentModel.DataAnnotations;

namespace ServiceWorkerWebsite.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int Service_Id { get; set; }
        public int Worke_Id { get; set; }
        public DateTime BookingDate { get; set; }
        
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public bool AgreeToTerms { get; set; }

        public virtual Service Service { get; set; }

        
        
        public DateTime BookingTime { get; set; }

        







    }
}
