using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ServiceWorkerWebsite.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ServiceWorkerWebsiteUser class
public class ServiceWorkerWebsiteUser : IdentityUser
{
    public string Firstname { get; set; }

    public string Lastname { get; set; }
}

