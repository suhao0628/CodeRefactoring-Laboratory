using AutoMapper;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Repository
{
    public class BasketService : IBasketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BasketService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<DishBasketDto>> GetBasket(Guid userId)
        {
            var baskets = await _context.Baskets.Where(b => b.UserId == userId).Include(w => w.Dish).ToListAsync();

            List<DishBasketDto> basketsDtos = new List<DishBasketDto>();
            foreach (var item in baskets)
            {
                DishBasketDto basketsDto = new()
                {
                    Id = item.DishId,
                    Name = item.Dish.Name,
                    Price = item.Dish.Price,
                    TotalPrice = item.Dish.Price * item.Amount,
                    Amount = item.Amount,
                    Image = item.Dish.Image
                };
                basketsDtos.Add(basketsDto);
            }
            return basketsDtos;
        }

        public async Task AddBasket(Guid dishId, Guid userId)
        {
            var dish = await _context.Dishes.Where(d => d.Id == dishId).FirstOrDefaultAsync();

            if (dish == null)
            {
                throw new NotFoundException();
            }

            var baskets =
                await _context.Baskets.Where(x => x.UserId == userId && x.DishId == dishId).FirstOrDefaultAsync();
            if (baskets == null)
            {
                Basket basket = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DishId = dish.Id
                };
                basket.Amount += 1;

                await _context.AddAsync(basket);
                await _context.SaveChangesAsync();
            }
            else
            {
                baskets.Amount += 1;
                _context.Update(baskets);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteBasket(Guid dishId, Guid userId,bool increase)
        {
            var dish = await _context.Dishes.Where(d => d.Id == dishId).FirstOrDefaultAsync();
            if (dish == null)
            {
                throw new NotFoundException();
            }
            var baskets = 
                await _context.Baskets.Where(x => x.UserId == userId && x.DishId == dishId).FirstOrDefaultAsync();

            if (baskets == null)
            {
                throw new NotFoundException();
            }

            if (baskets.Amount == 1 || !increase)
            { 
                _context.Baskets.Remove(baskets);
                await _context.SaveChangesAsync();
            }
            else
            {
                baskets.Amount -= 1;
                _context.Update(baskets);
                await _context.SaveChangesAsync();
            }
        }
    }
}
