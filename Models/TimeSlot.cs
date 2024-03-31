using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("TimeSlot_List")]
public class TimeSlot
{
    [Key]
    public int TimeSlotId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; } = false; // To track if the slot is already booked

    // Foreign Key
    public int Worke_Id { get; set; }

    // Navigation Property
    public virtual Worker Worker { get; set; }
}
