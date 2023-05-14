using Delivery.Api.Model.Dto;

namespace Delivery.Api.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderDto> GetOrderDetails(Guid orderId);
        Task<List<OrderInfoDto>> GetOrders(Guid userId);
        Task CreateOrder(OrderCreateDto orderCreateDto, Guid userId);
        Task ConfirmDelivery(Guid orderId, Guid userId);
    }
}
