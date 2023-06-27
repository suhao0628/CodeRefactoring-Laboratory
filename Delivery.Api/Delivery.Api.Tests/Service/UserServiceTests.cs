using Delivery.Api.Model.Entity;
using Delivery.Api.Model;
using Microsoft.EntityFrameworkCore;
using Delivery.Api.Data;
using Delivery.Api.Service;
using FluentAssertions;
using Delivery.Api.Exceptions;
using AutoMapper;
using Delivery.Api.Model.Dto;
using Delivery.Api.Service.Interfaces;
using FakeItEasy;
using Delivery.Api.Model.Enum;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery.Api.Tests.Service
{
    public class UserServiceTests
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
            });
            _mapper = config.CreateMapper();

            _jwtService = A.Fake<IJwtService>();
        }

        [Fact]
        public async Task Login_WithValidUser_ShouldReturnTokenRespoe()
        {
            // Arrange
            var loginCredentials = new LoginCredentials
            {
                Email = "test@example.com",
                Password = "password"
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Add a test user to the in-memory database
                var user = new User
                {
                    Email = loginCredentials.Email,
                    Password = loginCredentials.Password,

                    Id = Guid.NewGuid(),
                    FullName = "SuHao",
                    Address = "Tomsk",
                    BirthDate = DateTime.Now.ToString(),
                    Gender = Gender.Male,
                    PhoneNumber = "0123456789",

                };
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var jwtService = A.Fake<IJwtService>();
                var userService = new UserService(context, _mapper, jwtService);

                var dummyToken = new JwtSecurityToken();
                A.CallTo(() => jwtService.GetToken(A<User>._)).Returns(dummyToken);


                // Act
                var result = await userService.Login(loginCredentials);

                // Assert
                
                result.Token.Should().NotBeNullOrEmpty();
                result.Should().BeOfType<TokenResponse>();
            }
        }

        [Fact]
        public async Task Login_WithInvalidUser_ShouldThrowBadRequestException()
        {
            // Arrange
            var loginCredentials = new LoginCredentials
            {
                Email = "test@example.com",
                Password = "password"
            };

            var userService = new UserService(_context, _mapper, _jwtService);

            // Act
            Func<Task> action = async () => await userService.Login(loginCredentials);

            // Assert
            await action.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ShouldThrowBadRequestException()
        {
            // Arrange
            var loginCredentials = new LoginCredentials
            {
                Email = "test@example.com",
                Password = "incorrectpassword"
            };

            var user = new User
            {
                Email = loginCredentials.Email,
                Password = "password",
                Id = Guid.NewGuid(),
                    FullName = "SuHao",

                    Address = "Tomsk",
                    BirthDate = DateTime.Now.ToString(),
                    Gender = Gender.Male,
                    PhoneNumber = "0123456789",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userService = new UserService(_context, _mapper, _jwtService);

            // Act
            Func<Task> action = async () => await userService.Login(loginCredentials);

            // Assert
            await action.Should().ThrowAsync<BadRequestException>();
        }

    }
}

