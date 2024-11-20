using Microsoft.AspNetCore.Identity.UI.Services;
using System;

namespace ServiceWorkerWebsite.Services
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, Action<EmailSettings> configure)
        {
            services.Configure<EmailSettings>(configure);
            services.AddTransient<IEmailSender, EmailService>();
            return services;
        }
    }
}