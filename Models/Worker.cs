using Microsoft.AspNetCore.Identity;
using ServiceWorkerWebsite.Areas.Identity.Data;
using ServiceWorkerWebsite.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Worker
{
    [Key]
    public int Worker_Id { get; set; }

    public string ProfilePic_Id { get; set; }



  

    public int Price { get; set; }

    // Foreign key property
    [ForeignKey("User")]
    public string UserId { get; set; }


    [ForeignKey("RoleId")]
    public IdentityRole Role { get; set; }
    public string RoleId { get; set; }

    // Navigation property to the associated user
    public virtual ServiceWorkerWebsiteUser User { get; set; }
    
    public virtual ICollection<WorkerService> WorkerServices { get; set; }

    public virtual ICollection<TimeSlot> AvailableTimeSlots { get; set; }
}
