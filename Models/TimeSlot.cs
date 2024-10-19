using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TimeSlot_List")]
public class TimeSlot
{
    [Key]
    public int TimeSlotId { get; set; }

    // Store as a comma-separated string
    public string SelectedDates { get; set; }

    

    // Store as a comma-separated string
    public string TimeSlots { get; set; }

    public int Worker_Id { get; set; }

    public virtual Worker Worker { get; set; }

    public bool IsBooked { get; set; } = false;
}
