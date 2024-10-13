using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceWorkerWebsite.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        public int? TimeSlotId { get; set; }

        [Required(ErrorMessage = "You must agree to terms")]
        public bool AgreeToTerms { get; set; }

        // Store the UserId (Foreign key from AspNetUsers table)
        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; } // Stores the logged-in user's ID

        // Foreign Key Relationships
        public int Service_Id { get; set; }
        [ForeignKey(nameof(Service_Id))]
        public virtual Service Service { get; set; }

        public int Worker_Id { get; set; }
        [ForeignKey(nameof(Worker_Id))]
        public virtual Worker Worker { get; set; }

        // Navigation property for TimeSlot
        public virtual TimeSlot TimeSlot { get; set; }
    }
}
