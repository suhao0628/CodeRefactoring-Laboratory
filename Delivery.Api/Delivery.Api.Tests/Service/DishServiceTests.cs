using AutoMapper;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Model.Enum;
using Delivery.Api.Service;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Tests.Service
{
    public  class DishServiceTests

    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DishServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dish, DishDto>();
            });
            _mapper = config.CreateMapper();
        }

        #region  GetDish Tests
        [Fact]
        public async Task GetDish_WithValidCategoryAndVegetarian_ShouldReturnFilteredDishes()
        {
            // Arrange
            var category = new DishCategory[] { DishCategory.Wok };
            var sorting = DishSorting.NameAsc;
            var vegetarian = true;
            var page = 1;

            var dishes = new List<Dish>
            {
                new Dish { Id = Guid.NewGuid(), Name = "Dish 1", Category = DishCategory.Wok, Vegetarian = true },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 2", Category = DishCategory.Wok, Vegetarian = false },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 3", Category = DishCategory.Soup, Vegetarian = true }
            };

            await _context
                .Dishes.AddRangeAsync(dishes);
            await _context.SaveChangesAsync();

            var repository = new DishService(_context, _mapper);

            // Act
            var result = await repository.GetDish(category, sorting, vegetarian, page);

            // Assert
            result.Should().NotBeNull(); 
            result.Should().BeOfType<DishPagedListDto>();
            result.Dishes.Should().HaveCount(1);
            result.Dishes.Should().OnlyContain(d => d.Category == DishCategory.Wok && d.Vegetarian);
        }

        [Fact]
        public async Task GetDish_WithInvalidCategoryAndVegetarian_ShouldThrowNotFoundException()
        {
            // Arrange
            var category = new DishCategory[] {};
            var sorting = DishSorting.NameAsc;
            var vegetarian = true;
            var page = 1;

            var dishes = new List<Dish>
            {
                new Dish { Id = Guid.NewGuid(), Name = "Dish 1", Category = DishCategory.Wok, Vegetarian = false },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 2", Category = DishCategory.Wok, Vegetarian = false },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 3", Category = DishCategory.Wok, Vegetarian = false }
            };

            await _context.Dishes.AddRangeAsync(dishes);
            await _context.SaveChangesAsync();

            var repository = new DishService(_context, _mapper);

            // Act
            Func<Task> act = async () => await repository.GetDish(category, sorting, vegetarian, page);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }


        [Fact]
        public async Task GetDish_WithSorting_ShouldReturnSortedDishes()
        {
            // Arrange
            var category = new DishCategory[] { DishCategory.Wok };
            var sorting = DishSorting.PriceDesc;
            var vegetarian = true;
            var page = 1;

            var dishes = new List<Dish>
            {
                new Dish { Id = Guid.NewGuid(), Name = "Dish 1", Category = DishCategory.Wok, Vegetarian = true, Price = 10 },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 2", Category = DishCategory.Wok, Vegetarian = true, Price = 20 },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 3", Category = DishCategory.Wok, Vegetarian = true, Price = 15 }
            };

            await _context.Dishes.AddRangeAsync(dishes);
            await _context.SaveChangesAsync();

            var repository = new DishService(_context, _mapper);

            // Act
            var result = await repository.GetDish(category, sorting, vegetarian, page);

            // Assert
            result.Should().NotBeNull();
            result.Dishes.Should().HaveCount(3);
            result.Dishes.Should().BeInDescendingOrder(d => d.Price);

            var dish2 = result.Dishes.FirstOrDefault(d => d.Price == 20);
            result.Dishes.IndexOf(dish2).Should().Be(0);
        }

        [Fact]
        public async Task GetDish_WithValidPage_ShouldReturnPagedDishes()
        {
            // Arrange
            var category = new DishCategory[] { DishCategory.Wok };
            var sorting = DishSorting.NameAsc;
            var vegetarian = true;
            var page = 2;

            var dishes = new List<Dish>
            {
                new Dish { Id = Guid.NewGuid(), Name = "Dish 1", Category = DishCategory.Wok, Vegetarian = true },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 2", Category = DishCategory.Wok, Vegetarian = true },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 3", Category = DishCategory.Wok, Vegetarian = true },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 4", Category = DishCategory.Wok, Vegetarian = true },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 5", Category = DishCategory.Wok, Vegetarian = true },
                new Dish { Id = Guid.NewGuid(), Name = "Dish 6", Category = DishCategory.Wok, Vegetarian = true },
            };

            await _context.Dishes.AddRangeAsync(dishes);
            await _context.SaveChangesAsync();

            var repository = new DishService(_context, _mapper);

            // Act
            var result = await repository.GetDish(category, sorting, vegetarian, page);

            // Assert
            result.Should().NotBeNull();
            result.Dishes.Should().HaveCount(1);
            result.Pagination.Should().NotBeNull();
            result.Pagination.Size.Should().Be(5); // Assuming PageSize is 5
            result.Pagination.Count.Should().Be(2); // Assuming there are 6 total dishes and PageSize is 5
            result.Pagination.Current.Should().Be(page);
        }

        #endregion

        #region GetDishDetails Tests
        [Fact]
        public async Task GetDishDetails_WithExistingId_ShouldReturnDishDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dish = new Dish { Id = id, Name = "Test Dish" };

            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();

            var repository = new DishService(_context, _mapper);

            // Act
            var result = await repository.GetDishDetails(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Name.Should().Be(dish.Name);
            
        }

        [Fact]
        public async Task GetDishDetails_WithNonExistingId_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var repository = new DishService(_context, _mapper);

            // Act
            Func<Task> act = async () => await repository.GetDishDetails(nonExistingId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
        #endregion

    }
}
