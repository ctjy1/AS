using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment2.Models; // Replace with your actual models namespace
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Assignment2.Model;
using Assignment2.ViewModels;

namespace Assignment2.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly AuthDbContext _dbContext; // Replace with your actual DbContext

        public ResetPasswordModel(AuthDbContext dbContext) => _dbContext = dbContext;

        [BindProperty]
        public ResetPassword ResetPassword { get; set; }

        public void OnGet(string token)
        {
            ResetPassword = new ResetPassword
            {
                Token = token
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var tokenRecord = await _dbContext.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == ResetPassword.Token && t.ExpiryDate > DateTime.UtcNow);

            if (tokenRecord?.User != null)
            {
                // Reset password logic here
                // Update the user's password in your database
                // Ensure to hash the new password before saving

                // Remove or invalidate the token
                _dbContext.PasswordResetTokens.Remove(tokenRecord);
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/ResetConfirmation");
            }

            ModelState.AddModelError("", "Invalid or expired token.");
            return Page();
        }
    }
}
