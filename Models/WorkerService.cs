using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class WorkerService
{
   


    public int Worker_Id { get; set; }
    public Worker Worker { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public int Service_Id { get; set; }


    public Service Service { get; set; }
}
