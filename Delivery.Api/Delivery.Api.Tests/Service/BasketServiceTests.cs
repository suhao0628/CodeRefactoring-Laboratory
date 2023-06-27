using Delivery.Api.Controllers;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Service.Interfaces;
using Delivery.Api.Service;
using FakeItEasy;
using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace Delivery.Api.Tests.Service
{
    public class BasketServiceTests
    {
        //// Set up the in-memory database context before each test
        //public async Task<ApplicationDbContext>DatabaseContext()
        //{
        //     var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase(databaseName: "TestDatabase")
        //        .Options;
        //    var context = new ApplicationDbContext(options);
        //    context.Database.EnsureCreated();
        //    return context;
        //}

        //private static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        //{
        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        //        .Options;
        //    return optionsBuilder;
        //}

        
        private readonly ApplicationDbContext _context;

        public BasketServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
        }

        #region GetBasket Tests
        [Fact]
        public async Task GetBasket_ShouldReturnListOfDishBasketDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dishId = Guid.NewGuid();
            var dish = new Dish { Id = dishId, Name = "Test Dish", Price = 10, Image = "test.jpg" };
            var basket = new Basket { UserId = userId, DishId = dishId, Amount = 2 };

            _context.Dishes.Add(dish);
            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();

            var basketService = new BasketService(_context);

            // Act
            var result = await basketService.GetBasket(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<DishBasketDto>>();
        }

        [Fact]
        public async Task GetBasket_ShouldReturnEmptyList_WhenUserIdNotExisting()
        {
            // Arrange
            var nonExistingUserId = Guid.NewGuid();
            var basketService = new BasketService(_context);

            // Act
            var result = await basketService.GetBasket(nonExistingUserId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        #endregion

        #region AddBasket
        [Fact]
        public async Task AddBasket_ShouldAddBasketToDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dishId = Guid.NewGuid();
            var dish = new Dish { Id = dishId, Name = "Test Dish", Price = 10, Image = "test.jpg" };
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            var basketService = new BasketService(_context);

            // Act
            await basketService.AddBasket(dishId, userId);

            // Assert
            var basket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == userId && b.DishId == dishId);
            basket.Should().NotBeNull();
            basket.Amount.Should().Be(1);
        }

        [Fact]
        public async Task AddBasket_ShouldReturnNotFound_WhenDataNotFound()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var userId = Guid.NewGuid();

           
            var basketService = new BasketService(_context);

            // Act
            Func<Task> result = async () => await basketService.AddBasket(dishId, userId);

            // Assert

            result.Should().NotBeNull();
            await result.Should().ThrowAsync<NotFoundException>();
        }
        #endregion

        #region DeleteBasket Tests
        [Fact]
        public async Task DeleteBasket_ShouldDeleteBasketFromDatabase()
        {
            // Arrange
            
            var userId = Guid.NewGuid();
            var dishId = Guid.NewGuid();
            var dish = new Dish { Id = dishId, Name = "Test Dish", Price = 10, Image = "test.jpg" };
            _context.Dishes.Add(dish);
            var basket = new Basket { UserId = userId, DishId = dishId, Amount = 2 };
            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();

            var basketService = new BasketService(_context);
            await basketService.DeleteBasket(dishId, userId, increase: true);

            //Act
            await basketService.DeleteBasket(dishId, userId, increase: false);

            // Assert
            var remainingBasket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == userId && b.DishId == dishId);
            
            remainingBasket.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBasket_ShouldReturnNotFound_WhenDataNotFound()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var basketService = new BasketService(_context);

            // Act
            Func<Task> result = async () => await basketService.DeleteBasket(dishId, userId, increase: true);

            // Assert
            await result.Should().ThrowAsync<NotFoundException>();
        }
        #endregion


    }
}
