using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
   
        public int? TimeSlotId { get; set; }  
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public bool AgreeToTerms { get; set; }
        [ForeignKey("Worker_Id")]
        public virtual Worker Worker { get; set; }
        public virtual Service Service { get; set; }

        

    }
}
