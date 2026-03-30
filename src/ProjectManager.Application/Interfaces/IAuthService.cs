using System.Threading.Tasks;

namespace ProjectManager.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(DTOs.RegisterDto dto);
        Task<(bool Success, string Token, string Email)> LoginAsync(DTOs.LoginDto dto);
    }
}
