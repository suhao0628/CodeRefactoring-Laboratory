using Delivery.Api.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Delivery.Api.Repository.Interfaces;
using Delivery.Api.Model;

namespace Delivery.Api.Controllers
{
    [Route("api/basket")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        public BasketController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        #region Get user cart
        /// <summary>
        /// Get user cart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(IEnumerable<DishBasketDto>),200)]
        [ProducesResponseType(typeof(void),401)]
        [ProducesResponseType(typeof(Response),500)]
        public async Task<IActionResult> GetBasket()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            return Ok(await _basketRepository.GetBasket(userId));
        }
        #endregion

        #region Add dish to cart
        /// <summary>
        /// Add dish to cart
        /// </summary>
        /// <param name="dishId"></param>
        /// <returns></returns>
        [HttpPost("dish/{dishId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> AddBasket(Guid dishId)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _basketRepository.AddBasket(dishId, userId);
            return Ok();
        }
        #endregion

        #region Descrease the number of dishes in the cart
        /// <summary>
        /// Descrease the number of dishes in the cart (if increase=true),or remove the dish completely(increase=false)
        /// </summary>
        /// <param name="dishId"></param>
        /// <param name="increase"></param>
        /// <returns></returns>
        /// 
        [HttpDelete("dish/{dishId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> DeleteBasket(Guid dishId, bool increase)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _basketRepository.DeleteBasket(dishId, userId, increase);
            return Ok();
        }
        #endregion

    }
}
