using Compass.Data.Data.Context;
using Compass.Data.Data.Interfaces;
using Compass.Data.Data.Models;
using Compass.Data.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Compass.Data.Data.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AppUser> FindByEmailAsync(LoginUserVM model)
        {
         
            var result = await _userManager.FindByEmailAsync(model.Email);
            return result;
        }

        public async Task<IdentityResult> RegisterUserAsync(AppUser model, string password)
        {
            var result = await _userManager.CreateAsync(model, password);
            return result;
        }

        public async Task<bool> ValidatePasswordAsync(LoginUserVM model, string password)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser appUser)
        {
            var result = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            return result;
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            var result = await _userManager.FindByIdAsync(id);
            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(AppUser model, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(model, token);
            return result;
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            return result;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(AppUser model)
        {
            var result = await _userManager.GeneratePasswordResetTokenAsync(model);
            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(AppUser model, string token, string password)
        {
            var result = await _userManager.ResetPasswordAsync(model, token, password);
            return result;
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            using (var _context = new AppDbContext())
            {
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RefreshToken> CheckRefreshTokenAsync(string refreshToken)
        {
            using(var _context = new AppDbContext())
            {
                var result = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
                return result;
            }
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            using (var _context = new AppDbContext())
            {
                _context.RefreshTokens.Update(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        async Task<IList<string>> IUserRepository.GetRolesAsync(AppUser? model)
        {
            var result = await _userManager.GetRolesAsync(model);
            
            return result;
        }

        public async Task<List<AppUser>> GetUsersAsync(int start, int end, bool isAll = false)
        {
            if (isAll)
            {
                return _userManager.Users.ToList();
            }
            return _userManager.Users.ToList().Take(new Range(new Index(start), new Index(end))).ToList();   
        }

        public async Task<int> GetUsersQuantityAsync()
        {
            var result = _userManager.Users.Count();
            return result;
        }

        public async Task<bool> SetUserRoleAsync(AppUser model, string role)
        {
            var res = await _userManager.AddToRoleAsync(model,role);
            if (res.Succeeded)
            {
                return true;
            }
            return false;
        }

        public async Task<AppUser> GetUserProfileAsync(string userId)
        {
            var result = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return result;
        }

        public async Task<IdentityResult> UpdateUserProfileAsync(AppUser model)
        {
            var result = await _userManager.UpdateAsync(model);
            return result;
        }

        public async Task<IdentityResult> DeleteUserForAdminAsync(AppUser model)
        {
            var result = await _userManager.DeleteAsync(model);
            return result;
        }

        public async Task<IdentityResult> UserEditForAdministratorAsync(AppUser model)
        {
            var result = await _userManager.UpdateAsync(model);
            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(AppUser model, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(model, currentPassword, newPassword);
            return result;
        }



        //public async Task<IdentityResult> BlockUserForAdminAsync(AppUser model)
        //{
        //    _userManager.SetLockoutEnabledAsync
        //}
    }
}
