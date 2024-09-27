namespace ServiceWorkerWebsite.Models
{
    public class WorkerDetailsViewModel
    {
        public Worker Worker { get; set; }
        public double AverageRating { get; set; }
        public List<Reviews> Reviews { get; set; }
    }
}
