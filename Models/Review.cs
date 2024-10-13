using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceWorkerWebsite.Controllers;

namespace ServiceWorkerWebsite.Models
{
    public class Reviews
    {
        [Key] // Indicates this is the primary key
        public int Id { get; set; }

        [ForeignKey("Worker_Id")] // Specifies the foreign key relationship
        public int Worker_Id { get; set; }

        [Required]
        public int RatingValue { get; set; }

        public string Comment { get; set; } // Optional text comment

        [Required]
        public DateTime ReviewDate { get; set; }

        // Navigation property to access the associated Worker
        public virtual Worker Worker { get; set; }
    }
}