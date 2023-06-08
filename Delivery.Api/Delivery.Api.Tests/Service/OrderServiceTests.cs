using AutoMapper;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Model.Enum;
using Delivery.Api.Service;
using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.Api.Tests.Service
{
    public  class OrderServiceTests
    {
        //// 单独的方法用于设置测试数据库
        //private ApplicationDbContext CreateTestDatabase()
        //{
        //    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase(databaseName: "TestDatabase")
        //        .Options;
        //    return new ApplicationDbContext(options);
        //}
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OrderCreateDto, Order>();
            });
            _mapper = config.CreateMapper();


        }
        #region CreateOrder Tests
        [Fact]
        public async Task CreateOrder_WithNonEmptyBaskets_ShouldCreateOrder()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderCreateDto = new OrderCreateDto();
            var baskets = new List<Basket>
            {
                new Basket { UserId = userId, Dish = new Dish { Id = Guid.NewGuid(), Price = 10, Image = "image1.jpg" ,Name="dish"}, Amount = 2 },
                new Basket { UserId = userId, Dish = new Dish { Id = Guid.NewGuid(), Price = 20, Image = "image2.jpg",Name = "dish"}, Amount = 3 }
            };
            await _context.Baskets.AddRangeAsync(baskets);
            await _context.SaveChangesAsync();

            var service = new OrderService(_context, _mapper);

            // Act
            await service.CreateOrder(orderCreateDto, userId);

            // Assert
            var createdOrder = await _context.Orders.FirstOrDefaultAsync();
            createdOrder.Should().NotBeNull();
            createdOrder.Price.Should().Be(80);
            createdOrder.Status.Should().Be(OrderStatus.InProcess);
        }

        [Fact]
        public async Task CreateOrder_WithEmptyBaskets_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderCreateDto = new OrderCreateDto();

            var repository = new OrderService(_context, _mapper);

            // Act
            Func<Task> act = async () => await repository.CreateOrder(orderCreateDto, userId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        #endregion
    }
}
