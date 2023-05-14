using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Model.Enum;

namespace Delivery.Api.Service.Interfaces
{
    public interface IDishService
    {
        Task<DishPagedListDto> GetDish(DishCategory[]? category, DishSorting? sorting, bool vegetarian, int page);
        Task<DishDto> GetDishDetails(Guid id);
        Task<bool> CheckRating(Guid id, Guid userId);
        Task SetRating(Guid id, int ratingScore, Guid userId);
    }
}
