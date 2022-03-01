using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public interface IUserService
    {
        Task<PaginatedList<User>> GetUsersAsync(PaginationFilter filter);
        Task<User> GetUserByIdAsync(string userId);
        Task<Result<User>> CreateUserAsync(User user, string password);
        Task<Result<User>> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
        Task<Result<User>> UpdateProfileImage(User user, Stream stream);
    }
}
