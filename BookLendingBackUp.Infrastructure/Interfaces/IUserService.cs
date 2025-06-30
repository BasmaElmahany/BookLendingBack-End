using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Infrastructure.Entities;

using System.Security.Claims;


namespace BookLendingBackUp.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterUserDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<string> ConfirmEmailAsync(string userId, string token);
        //Task<string> GenerateJwtToken(ApplicationUser user);

        //Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user);
    }
}
