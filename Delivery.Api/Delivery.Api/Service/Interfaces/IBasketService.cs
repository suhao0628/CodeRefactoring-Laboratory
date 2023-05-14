using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;

namespace Delivery.Api.Service.Interfaces
{
    public interface IBasketService
    {
        Task<List<DishBasketDto>> GetBasket(Guid userId);
        Task AddBasket(Guid dishId, Guid userId);
        Task DeleteBasket(Guid dishId, Guid userId, bool increase = false);
    }
}
