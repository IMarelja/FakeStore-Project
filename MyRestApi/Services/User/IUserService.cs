using MyRestApi.DTO.User;

namespace MyRestApi.Services;

public interface IUserService
{
    Task<List<UserReadDto>> GetAllAsync();
    Task<UserReadDto?> GetByIdAsync(int userId);
    Task<UserReadDto?> UpdateAsync(int userId, UserUpdateDto req);
    Task<bool> DeleteAsync(int userId);
}
