using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Worker
{
    [Key]
    public int Worke_Id { get; set; }
    public string Name { get; set; }
    public string Speciality { get; set; }
    public string Availability_Status { get; set; } 
    public double Ratings { get; set; }
    public string Reviews { get; set; }

    public int Price { get; set; }  

    [ForeignKey("Service")]
    public int Service_Id { get; set; }








    public virtual ICollection<TimeSlot> AvailableTimeSlots { get; set; }

    public virtual Service Service { get; set; }


    





}
