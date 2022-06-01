using PaketMan.Models.Api;

namespace PaketMan.Contracts
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);

        Task<AuthenticationResult> ResetPasswordAsync(string email, string password, int userId);
    }
}
