using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public class UserService: IUserService
    {
        private readonly DataContext context;
        private readonly IPaginationService paginationService;
        private readonly UserManager<User> userManager;

        public UserService(DataContext context, IPaginationService paginationService, UserManager<User> userManager)
        {
            this.context = context ?? throw new ArgumentNullException("context");
            this.paginationService = paginationService ?? throw new ArgumentNullException("paginationService");
            this.userManager = userManager ?? throw new ArgumentNullException("userManager");
        }

        public async Task<PaginatedList<User>> GetUsersAsync(PaginationFilter filter)
        {
            var query = context.Users.OrderBy(u => u.Email).AsQueryable();
            var result = await paginationService.GetPaginatedListAsync(query, filter);
            foreach (var user in result.Data)
            {
                var roles = await userManager.GetRolesAsync(user);
                user.Roles = roles.ToArray();
            }
            return result;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;
            var roles = await userManager.GetRolesAsync(user);
            user.Roles = roles.ToArray();
            return user;
        }

        public async Task<Result<User>> CreateUserAsync(User user, string password)
        {
            user.UserName = user.Email;
            user.EmailConfirmed = true;
            var userCreationResult = await userManager.CreateAsync(user, password);
            if (!userCreationResult.Succeeded)
            {
                return new Result<User> { Errors = userCreationResult.Errors.Select(e => e.Description) };
            }
            var roleResult = await userManager.AddToRolesAsync(user, user.Roles);
            if (!roleResult.Succeeded)
            {
                return new Result<User> { Errors = roleResult.Errors.Select(e => e.Description) };
            }

            var roles = await userManager.GetRolesAsync(user);
            user.Roles = roles.ToArray();
            return new Result<User> { Data = user };
        }

        public async Task<Result<User>> UpdateUserAsync(User user)
        {
            context.Users.Update(user);
            var updated = await context.SaveChangesAsync();
            if (updated <= 0)
            {
                return new Result<User> { Errors = new string[] { "User update failed!" } };
            }
            var currentRoles = await userManager.GetRolesAsync(user);
            var newRoles = user.Roles.Where(ur => !currentRoles.Contains(ur));
            var rolesToDelete = currentRoles.Where(r => !user.Roles.Contains(r));
            var roleResult = await userManager.AddToRolesAsync(user, newRoles);
            if (!roleResult.Succeeded)
            {
                return new Result<User> { Errors = roleResult.Errors.Select(e => e.Description) };
            }
            roleResult = await userManager.RemoveFromRolesAsync(user, rolesToDelete);
            if (!roleResult.Succeeded)
            {
                return new Result<User> { Errors = roleResult.Errors.Select(e => e.Description) };
            }

            var roles = await userManager.GetRolesAsync(user);
            user.Roles = roles.ToArray();
            return new Result<User> { Data = user };
        }

        public async Task<Result<User>> UpdateProfileImage(User user, Stream stream)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    user.ProfileImage = memoryStream.ToArray();
                    context.Update(user);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return new Result<User> { Errors = new string[] { e.Message } };
            }
            return new Result<User> { Data = user };
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            var refreshTokens = await context.RefreshTokens.Where(r => r.User.Id == user.Id).ToListAsync();
            context.RefreshTokens.RemoveRange(refreshTokens);
            var userRoles = await context.UserRoles.Where(r => r.UserId == user.Id).ToListAsync();
            context.UserRoles.RemoveRange(userRoles);
            var apartments = await context.Apartments.Where(a => a.Realtor.Id == user.Id).ToListAsync();
            context.Apartments.RemoveRange(apartments);
            context.Users.Remove(user);
            var deleted = await context.SaveChangesAsync();
            return deleted > 0;
        }
    }
}
