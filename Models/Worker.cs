using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Worker
{
    [Key]
    public int Worker_Id { get; set; }
    public string ProfilePic_Id { get; set; }
    public string Name { get; set; }
    public string Availability_Status { get; set; }
    public double Ratings { get; set; }
    public string Reviews { get; set; }
    public int Price { get; set; }


    public virtual ICollection<WorkerService> WorkerServices { get; set; }

    public virtual ICollection<TimeSlot> AvailableTimeSlots { get; set; }

   // public virtual Service Service { get; set; }
}
