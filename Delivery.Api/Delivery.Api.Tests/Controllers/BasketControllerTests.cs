using Delivery.Api.Controllers;
using Delivery.Api.Model.Dto;
using Delivery.Api.Service.Interfaces;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Delivery.Api.Tests.Controllers
{
    public class BasketControllerTests
    {
        private readonly IBasketService _basketService;
        private readonly BasketController _basketController;

        public BasketControllerTests()
        {
            _basketService = A.Fake<IBasketService>();
            _basketController = new BasketController(_basketService);
        }

        private BasketController CreateHttpContext(Guid userId)
        {
            var fakeHttpContext = new DefaultHttpContext();
            fakeHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim("UserId", userId.ToString())
            }));
            _basketController.ControllerContext = new ControllerContext
            {
                HttpContext = fakeHttpContext
            };
            return _basketController;
        }

        [Fact]
        public async Task GetBasket_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var controller = CreateHttpContext(userId);
            var expectedDishBasketDtos = A.Fake<List<DishBasketDto>>();
            A.CallTo(() => _basketService.GetBasket(userId)).Returns(expectedDishBasketDtos);

            // Act
            var result = await controller.GetBasket();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AddBasket_ReturnsOk()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var controller = CreateHttpContext(userId);

            // Act
            var result = await controller.AddBasket(dishId);

            // Assert
            result.Should().BeOfType<OkResult>();
            A.CallTo(() => _basketService.AddBasket(dishId, userId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DeleteBasket_ReturnsOk()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var controller = CreateHttpContext(userId);

            // Act
            var result = await controller.DeleteBasket(dishId, increase: true);

            // Assert
            result.Should().BeOfType<OkResult>();
            A.CallTo(() => _basketService.DeleteBasket(dishId, userId, true)).MustHaveHappenedOnceExactly();
        }

    }
}
