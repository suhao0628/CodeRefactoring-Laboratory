using AutoMapper;
using AutoMapper.Internal;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Entity;
using Delivery.Api.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Delivery.Api.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtRepository _jwtRepository;
        public UserRepository(ApplicationDbContext context,IMapper mapper, IJwtRepository jwtRepository)
        {
            _context = context;
            _mapper = mapper;
            _jwtRepository = jwtRepository;
        }
        public async Task<TokenResponse> Register(UserRegisterModel register)
        {
            if (await _context.Users.Where(x => register.Email == x.Email).FirstOrDefaultAsync() != null) {
                throw new BadRequestException();
            }
            User user = new()
            {
                Id = Guid.NewGuid(),
                FullName = register.FullName,
                Email = register.Email,
                Password = register.Password,
                Address = register.Address,
                BirthDate = register.BirthDate,
                Gender = register.Gender,
                PhoneNumber = register.PhoneNumber
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(_jwtRepository.GetToken(user))
            };
        }
        public async Task<TokenResponse> Login(LoginCredentials login)
        {
            var user = await _context.Users.Where(u => u.Email == login.Email).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new BadRequestException();
            }
            if (user.Password != login.Password)
            {
                throw new BadRequestException();
            }
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(_jwtRepository.GetToken(user))
            };
        }
        
        public async Task<UserDto> GetProfile(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedException();
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task EditProfile(UserEditModel profile, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedException();
            }
            
            user.FullName = profile.FullName;
            user.BirthDate = profile.BirthDate;
            user.Gender = profile.Gender;
            user.Address = profile.Address;
            user.PhoneNumber = profile.PhoneNumber;
            
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}