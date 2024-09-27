using Microsoft.AspNetCore.Identity;
namespace ServiceWorkerWebsite.Models
{
	public class Applicationuser:IdentityUser
	{
		public string Firstname { get; set; }

		public string Lastname { get; set; }
	}
}

