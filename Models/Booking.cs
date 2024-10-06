using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceWorkerWebsite.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Service))] // Reference the navigation property "Service"
        public int Service_Id { get; set; }

        [ForeignKey(nameof(Worker))] // Reference the navigation property "Worker"
        public int Worker_Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        public int? TimeSlotId { get; set; }  // Foreign key to TimeSlot (optional)

        [Required(ErrorMessage = "You must agree to terms")]
        public bool AgreeToTerms { get; set; }

        // Navigation properties
        public virtual Worker Worker { get; set; } = new Worker(); // Initialize to avoid null references
        public virtual Service Service { get; set; } = new Service(); // Initialize to avoid null references
    }
}
