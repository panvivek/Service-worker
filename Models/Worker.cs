using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Worker
{
    [Key]public int Worke_Id { get; set; }
    public string Name { get; set; }
    public string Speciality { get; set; }
    public string Availability_Status { get; set; } 
    public double Ratings { get; set; }
    public string Reviews { get; set; }

    
    
}
