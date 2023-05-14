using Delivery.Api.Model;
using Delivery.Api.Model.Dto;

namespace Delivery.Api.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<TokenResponse> Register(UserRegisterModel register);
        Task<TokenResponse> Login(LoginCredentials credentials);

        Task<UserDto> GetProfile(Guid userId);
        Task EditProfile(UserEditModel userEditModel, Guid userId);
    }
}
