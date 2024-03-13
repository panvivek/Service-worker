using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class ServiceController : Controller
{
    public IActionResult Services()
    {
        List<Service> services = new List<Service>
        {
            new Service { Id = 1, Name = "Plumbing", ImageUrl = "/images/plumbing.jpg", Description = "We offer comprehensive plumbing services including installation, repair, and maintenance of pipes, fixtures, and fittings." },
            new Service { Id = 2, Name = "Electrician", ImageUrl = "/images/electrician.jpg", Description = "Our electricians are skilled in electrical installation, repair, and troubleshooting for residential and commercial properties." },
            new Service { Id = 3, Name = "Beautician", ImageUrl = "/images/beautician.jpg", Description = "Our beauticians provide a range of beauty services including skincare, haircare, and makeup application." },
            new Service { Id = 4, Name = "AC Repair", ImageUrl = "/images/ac-repair.jpg", Description = "Our technicians specialize in repairing and maintaining air conditioning systems for homes and businesses." },
            new Service { Id = 5, Name = "Carpentry", ImageUrl = "/images/carpentry.jpg", Description = "Our carpenters offer custom woodworking services including furniture making, cabinetry, and trim installation." },
            new Service { Id = 6, Name = "Painting", ImageUrl = "/images/painting.jpg", Description = "We provide professional painting services for both interior and exterior surfaces, delivering high-quality finishes." },
            new Service { Id = 7, Name = "Cleaning Services", ImageUrl = "/images/cleaning.jpg", Description = "Our cleaning services cover residential and commercial properties, ensuring a clean and hygienic environment." },
            new Service { Id = 8, Name = "Pest Control", ImageUrl = "/images/pest-control.jpg", Description = "Our pest control services effectively eliminate pests such as insects, rodents, and termites, protecting your property." },
            new Service { Id = 9, Name = "Appliance Repair", ImageUrl = "/images/appliance-repair.jpg", Description = "We repair and service a wide range of household appliances including refrigerators, washing machines, and ovens." },
            new Service { Id = 10, Name = "Home Security Services", ImageUrl = "/images/home-security.jpg", Description = "Our home security services include installation and monitoring of alarm systems, CCTV cameras, and smart locks." },
            new Service { Id = 11, Name = "Landscaping/Gardening", ImageUrl = "/images/landscaping.jpg", Description = "Our landscaping and gardening services enhance outdoor spaces with features such as gardens, lawns, and hardscapes." },
            new Service { Id = 12, Name = "Roofing Services", ImageUrl = "/images/roofing.jpg", Description = "We provide roofing solutions including installation, repair, and replacement of roofs using high-quality materials." },
            new Service { Id = 13, Name = "Flooring Installation/Repair", ImageUrl = "/images/flooring.jpg", Description = "Our flooring services cover installation and repair of various flooring materials such as hardwood, tile, and laminate." },
            new Service { Id = 14, Name = "Interior Design", ImageUrl = "/images/interior-design.jpg", Description = "Our interior designers create functional and aesthetically pleasing interiors tailored to your style and preferences." },
            new Service { Id = 15, Name = "Moving Services", ImageUrl = "/images/moving.jpg", Description = "We offer professional moving services including packing, transportation, and unpacking, ensuring a smooth relocation process." },
            new Service { Id = 16, Name = "Car Washing/Detailing", ImageUrl = "/images/car-washing.jpg", Description = "Our car washing and detailing services restore the appearance of vehicles, leaving them clean and polished." },
            new Service { Id = 17, Name = "Computer Repair/IT Services", ImageUrl = "/images/computer-repair.jpg", Description = "Our IT specialists provide computer repair, maintenance, and support services for individuals and businesses." },
            new Service { Id = 18, Name = "Legal Services", ImageUrl = "/images/legal-services.jpg", Description = "We offer a wide range of legal services including legal advice, document preparation, and representation in legal matters." },
            new Service { Id = 19, Name = "Financial Services", ImageUrl = "/images/financial-services.jpg", Description = "Our financial services cover areas such as financial planning, investment management, and tax preparation, helping clients achieve their financial goals." },
            new Service { Id = 20, Name = "Personal Training/Fitness Services", ImageUrl = "/images/personal-training.jpg", Description = "Our personal trainers provide customized fitness programs and one-on-one training sessions to help clients achieve their fitness objectives." },
            new Service { Id = 21, Name = "Childcare/Babysitting", ImageUrl = "/images/childcare.jpg", Description = "Our childcare services offer a safe and nurturing environment for children, including educational activities and supervision." },
            new Service { Id = 22, Name = "Tutoring/Educational Services", ImageUrl = "/images/tutoring.jpg", Description = "We provide tutoring and educational services for students of all ages, covering various subjects and academic levels." },
            new Service { Id = 23, Name = "Photography/Videography", ImageUrl = "/images/photography.jpg", Description = "Our photography and videography services capture memorable moments and events with creativity and professionalism." },
            new Service { Id = 24, Name = "Event Planning/Management", ImageUrl = "/images/event-planning.jpg", Description = "Our event planning and management services cover all aspects of event coordination, ensuring successful and memorable occasions." },
            new Service { Id = 25, Name = "Mobile Phone Repair", ImageUrl = "/images/mobile-phone-repair.jpg", Description = "We repair and service mobile phones and tablets, addressing issues such as screen damage, battery replacement, and software troubleshooting." }
        };

        return View(services);
    }
}
