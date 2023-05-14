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
            var carts = await _context.Carts.Where(w => w.OrderId == orderId).Include(w => w.Order).ToListAsync();
            if (!carts.Any())
            {
                throw new NotFoundException();
            }

            OrderDto orderDto = new()
            {
                Id = carts.FirstOrDefault().OrderId,
                DeliveryTime = carts.FirstOrDefault().Order.DeliveryTime,
                OrderTime = carts.FirstOrDefault().Order.OrderTime,
                Status = carts.FirstOrDefault().Order.Status,
                Price = carts.FirstOrDefault().Price,
                Dishes = _mapper.Map<List<DishBasketDto>>(carts),
                Address = carts.FirstOrDefault().Order.Address
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
            var baskets = await _context.Baskets.Where(b => b.UserId == userId).Include(b => b.Dish).ToListAsync();
            if (baskets.Any())
            {
                Order order = _mapper.Map<Order>(orderCreateDto);

                int allTotalPrice = 0;

                List<Cart> carts = new List<Cart>();

                foreach (var basketItem in baskets)
                {
                    allTotalPrice += basketItem.Dish.Price * basketItem.Amount;

                    //Migration of data from basket to cart
                    Cart cart = new()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        DishId = basketItem.Dish.Id,
                        Name = basketItem.Dish.Name,
                        Price = basketItem.Dish.Price,
                        TotalPrice = basketItem.Dish.Price * basketItem.Amount,
                        Amount = basketItem.Amount,
                        Image = basketItem.Dish.Image
                    };
                    carts.Add(cart);
                }

                order.Price = allTotalPrice;
                order.OrderTime = DateTime.Now;
                order.Status = OrderStatus.InProcess;

                await _context.AddRangeAsync(carts);
                await _context.AddAsync(order);
                //Delete basket
                _context.RemoveRange(baskets);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException();
            }
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
    }
}