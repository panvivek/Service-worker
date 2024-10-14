using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ServiceWorkerWebsite.Models;

public class Service
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Service_Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }

    public virtual ICollection<WorkerService> WorkerServices { get; set; }
    public virtual ICollection<Reviews> Review { get; set; }

}
