using AutoMapper;
using Delivery.Api.Data;
using Delivery.Api.Exceptions;
using Delivery.Api.Model;
using Delivery.Api.Model.Dto;
using Delivery.Api.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery.Api.Controllers
{
    [Route("api/account")]
	[ApiController]
	public class UserController : ControllerBase
	{
        private readonly IUserService _userRepository;
        private readonly IDistributedCache _cache;

        public UserController(IUserService userRepository,IDistributedCache cache)
		{
			_userRepository = userRepository;
            _cache = cache;
        }

        #region Register 
        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] UserRegisterModel register)
        {
            return await _userRepository.Register(register);
        }
        #endregion

        #region Log in
        /// <summary>
        /// Log in to the system
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("login")]
		[AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        //[ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<TokenResponse> Login([FromBody] LoginCredentials login)
		{
            return await _userRepository.Login(login);
        }
        #endregion

        #region Log out
        /// <summary>
        /// Log out system user
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
		[AllowAnonymous]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> Logout()
		{
            try
            {
                var auth = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer"))
                {
                    var token = auth.Substring("Bearer".Length).Trim();

                    await _cache.SetStringAsync(token, "1", new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                    });
                }
                return Ok();
            }
            catch
            {
                return Unauthorized();
            }
        }
        #endregion

        #region Get user profile
        /// <summary>
        /// Get user profile
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("profile")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetProfile()
		{
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            return Ok(await _userRepository.GetProfile(userId));
        }
        #endregion

        #region Edit user profile
        /// <summary>
        /// Edit user profile
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPut("profile")]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> EditProfile([FromBody] UserEditModel profile)
		{
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _userRepository.EditProfile(profile, userId);
            return Ok();
        }
		#endregion

	}
}