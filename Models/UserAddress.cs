using ServiceWorkerWebsite.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceWorkerWebsite.Models
{
    public class UserAddress
    {
        [Key]
        public int UserAdd_Id { get; set; }

        // Store the UserId (Foreign key from AspNetUsers table)
        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; } // Stores the logged-in user's ID

        [Required]
        [MaxLength(200)]
        public string StreetNumberName { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string Province { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        // Navigation property to the associated user
        public virtual ServiceWorkerWebsiteUser User { get; set; }
    }
}
