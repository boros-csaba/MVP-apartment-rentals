using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(Email email);
    }
}
