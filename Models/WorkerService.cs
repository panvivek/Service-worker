using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class WorkerService
{
    [Key]
    public int WorkerServiceId { get; set; }


    public int Worker_Id { get; set; }
    public Worker Worker { get; set; }

    public int Service_Id { get; set; }


    public Service Service { get; set; }
}
