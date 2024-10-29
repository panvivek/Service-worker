namespace ServiceWorkerWebsite.Models
{
    internal class WorkerDetailsViewModel
    {
        public object Worker_Id { get; set; }
        public object ProfilePicUrl { get; set; }
        public object Price { get; set; }
        public object Reviews { get; set; }

        public int ServiceId { get; set; } // To track the current service
        public List<ReviewViewModel> Reviewss { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


    }

    public class ReviewViewModel
    {

        public int RatingValue { get; set; } // Rating value
        public string Comment { get; set; } // Comment from customer
        public DateTime ReviewDate { get; set; } // Date of review
        public string CustomerName { get; set; } // Customer's name

    }

}