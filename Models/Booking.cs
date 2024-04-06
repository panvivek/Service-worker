using System.ComponentModel.DataAnnotations;

namespace ServiceWorkerWebsite.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
       
        public int Service_Id { get; set; }
        public int Worker_Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingDate { get; set; } = DateTime.Today;
   
        
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public bool AgreeToTerms { get; set; }

        public virtual Service Service { get; set; }

        

    }
}
