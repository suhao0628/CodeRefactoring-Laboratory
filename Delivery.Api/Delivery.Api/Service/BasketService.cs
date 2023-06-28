using AutoMapper;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Service
{
    public class BasketService : IBasketService
    {
        private readonly ApplicationDbContext _context;
        //private readonly IMapper _mapper;
        public BasketService(ApplicationDbContext context)
        {
            _context = context;
            //_mapper = mapper;
        }
        public async Task<List<DishBasketDto>> GetBasket(Guid userId)
        {
            var baskets = await _context.Baskets
                .Where(b => b.UserId == userId)
                .Select(b=>CreateDishBasketDto(b))
                .ToListAsync();

            return baskets;
        }
        private DishBasketDto CreateDishBasketDto(Basket basket)
        {
            return new DishBasketDto
            {
                Id = basket.DishId,
                Name = basket.Dish.Name,
                Price = basket.Dish.Price,
                TotalPrice = basket.Dish.Price * basket.Amount,
                Amount = basket.Amount,
                Image = basket.Dish.Image
            };
        }

        public async Task AddBasket(Guid dishId, Guid userId)
        {
            var dish = await GetDishById(dishId);

            var basket = await _context.Baskets
                .FirstOrDefaultAsync(x => x.UserId == userId && x.DishId == dishId);

            if (basket == null)
            {
                basket = new Basket
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DishId = dish.Id,
                    Amount = 1
                };
                await _context.AddAsync(basket);
            }
            else
            {
                basket.Amount += 1;
                _context.Update(basket);
            }

            await _context.SaveChangesAsync();
        }
        public async Task DeleteBasket(Guid dishId, Guid userId,bool increase)
        {
            var dish = await GetDishById(dishId);

            var basket = await _context.Baskets
                .FirstOrDefaultAsync(x => x.UserId == userId && x.DishId == dishId);

            if (basket == null)
            {
                throw new NotFoundException();
            }

            if (basket.Amount == 1 || !increase)
            {
                _context.Baskets.Remove(basket);
            }
            else
            {
                basket.Amount -= 1;
                _context.Update(basket);
            }

            await _context.SaveChangesAsync();
        }
        private async Task<Dish> GetDishById(Guid dishId)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);

            if (dish == null)
            {
                throw new NotFoundException();
            }

            return dish;
        }
    }
}
