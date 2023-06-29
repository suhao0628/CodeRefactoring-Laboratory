using AutoMapper;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Model.Enum;
using Delivery.Api.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;

namespace Delivery.Api.Service
{
    public class DishService : IDishService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private const int PageSize = AppConfig.PageSize;

        public DishService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<DishPagedListDto> GetDish(DishCategory? category, DishSorting? sorting, bool vegetarian, int page)
        {
            IQueryable<Dish> dishQueryable = _context.Dishes;
            dishQueryable = GetVegetarian(dishQueryable, vegetarian);
            dishQueryable = GetCategory(dishQueryable, category);

            int pageTotal = CalculatePageTotal(dishQueryable);
            page = GetValidPage(page, pageTotal);
            var dishes = await GetDishesForPage(dishQueryable, page);

            dishes = Sort(dishes, sorting);

            return GetDishPagedListDto(dishes, pageTotal, page);
        }
        private IQueryable<Dish> GetVegetarian(IQueryable<Dish> dishQueryable, bool vegetarian)
        {
            return dishQueryable.Where(d => d.Vegetarian == vegetarian);
        }

        private IQueryable<Dish> GetCategory(IQueryable<Dish> dishQueryable, DishCategory? category)
        {
            if (category==null)
            {
                throw new NotFoundException();
            }

            return dishQueryable.Where(x => category != null && x.Category == category.Value);
        }
        private int CalculatePageTotal(IQueryable<Dish> dishQueryable)
        {
            int dishCount = dishQueryable.Count();
            int pageTotal = (int)Math.Ceiling(dishCount / (double)PageSize);
            if (pageTotal == 0)
            {
                pageTotal = 1;
            }
            return pageTotal;
        }

        private int GetValidPage(int page, int pageTotal)
        {
            if (page < 1)
            {
                page = 1;
            }
            else if (page > pageTotal)
            {
                page = pageTotal;
            }
            return page;
        }

        private async Task<List<Dish>> GetDishesForPage(IQueryable<Dish> dishQueryable, int page)
        {
            var dishes = await dishQueryable.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
            return dishes;
        }
        private DishPagedListDto GetDishPagedListDto(List<Dish> dishes, int pageTotal, int page)
        {
            PageInfoModel paginationModel = new()
            {
                Size = PageSize,
                Count = pageTotal,
                Current = page
            };

            DishPagedListDto dishListDto = new()
            {
                Dishes = _mapper.Map<List<DishDto>>(dishes),
                Pagination = paginationModel
            };
            return dishListDto;
        }

        private static List<Dish> Sort(List<Dish> dishes, DishSorting? sorting)
        {
            switch (sorting)
            {
                case DishSorting.NameAsc: 
                    return dishes.OrderBy(d => d.Name).ToList();
                case DishSorting.NameDesc:
                    return dishes.OrderByDescending(d => d.Name).ToList();
                case DishSorting.PriceAsc:
                    return dishes.OrderBy(d => d.Price).ToList();
                case DishSorting.PriceDesc:
                    return dishes.OrderByDescending(d => d.Price).ToList();
                case DishSorting.RatingAsc:
                    return dishes.OrderBy(d => d.Rating).ToList();
                case DishSorting.RatingDesc:
                    return dishes.OrderByDescending(d => d.Rating).ToList();
                default:
                    throw new Exception();
            }
        }
        
        public async Task<DishDto> GetDishDetails(Guid id)
        {
            var dish = await _context.Dishes.Where(d => d.Id == id).FirstOrDefaultAsync();

            if (dish == null) {
                throw new NotFoundException();
            }
            return _mapper.Map<DishDto>(dish);
        }
        
        public async Task<bool> CheckRating(Guid id,Guid userId)
        {
            var dish =  _context.Dishes.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (dish == null)
            {
                throw new NotFoundException();
            }
            var carts = await _context.Carts.Where(x => x.DishId == id).FirstOrDefaultAsync();

            return await _context.Orders.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == carts.OrderId) != null;
        }
        
        public async Task SetRating(Guid id, int ratingScore, Guid userId)
        {
            var dish = await _context.Dishes.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (dish == null)
            {
                throw new NotFoundException();
            }
            if (!await CheckRating(id, userId))
            {
                throw new Exception();
            }

            var rating = await _context.Ratings.Where(r => r.UserId == userId && r.DishId == id)
                    .FirstOrDefaultAsync();
            if (rating != null)
            {
                rating.RatingScore = ratingScore;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    Id = Guid.NewGuid(),
                    DishId = id,
                    UserId = userId,
                    RatingScore = ratingScore
                });
            }
            await _context.SaveChangesAsync();
        }
    }
}
