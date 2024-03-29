using System.ComponentModel.DataAnnotations;

public class Service
{
    [Key]
    public int Service_Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; } 
}
