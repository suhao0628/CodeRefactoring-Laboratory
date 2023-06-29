using AutoMapper;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Model.Enum;
using Delivery.Api.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Service
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetOrderDetails(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException();
            }

            var baskets = await GetListBasket(order.UserId);

            OrderDto orderDto = new()
            {
                Id = order.Id,
                DeliveryTime = order.DeliveryTime,
                OrderTime = order.OrderTime,
                Status = order.Status,
                Price = order.Price,
                Dishes = _mapper.Map<List<DishBasketDto>>(baskets),
                Address = order.Address
            };

            return orderDto;
        }


        public async Task<List<OrderInfoDto>> GetOrders(Guid userId)
        {
            var orders = await _context.Orders.Where(x => x.UserId == userId).ToListAsync();

            return _mapper.Map<List<OrderInfoDto>>(orders);
        }

        public async Task CreateOrder(OrderCreateDto orderCreateDto, Guid userId)
        {
            var baskets = await GetListBasket(userId);


            Order order = _mapper.Map<Order>(orderCreateDto);

            int allTotalPrice = 0;

            foreach (var basketItem in baskets)
            {
                allTotalPrice += basketItem.Dish.Price * basketItem.Amount;
            }

            order.Price = allTotalPrice;
            order.OrderTime = DateTime.Now;
            order.Status = OrderStatus.InProcess;

            _context.Add(order);

            foreach (var basketItem in baskets)
            {
                _context.Remove(basketItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ConfirmDelivery(Guid orderId, Guid userId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException();
            }

            if (order.UserId != userId)
            {
                throw new ForbiddenException();
            }

            order.Status = OrderStatus.Delivered;
            _context.Update(order);
            await _context.SaveChangesAsync();
        }
        private async Task<List<Basket>> GetListBasket(Guid userId)
        {
            var baskets = await _context.Baskets.Where(b => b.UserId == userId)
                .Include(b => b.Dish)
                .ToListAsync();

            if (!baskets.Any())
            {
                throw new NotFoundException();
            }
            return baskets;
        }
    }
}