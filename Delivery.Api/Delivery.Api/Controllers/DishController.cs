using Delivery.Api.Data;
using Delivery.Api.Model;
using Delivery.Api.Model.Dto;
using Delivery.Api.Model.Enum;
using Delivery.Api.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Api.Controllers
{
    [Route("api/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishRepository _dishRepository;

        public DishController(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        #region list of dishes(menu)
        /// <summary>
        /// Get a list of dishes(menu)
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="sorting"></param>
        /// <param name="vegetarian"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DishPagedListDto>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetDish([FromQuery] DishCategory[]? categories, [FromQuery]DishSorting? sorting,
            bool vegetarian, int page = 1)
        {
            return Ok(await _dishRepository.GetDish(categories, sorting, vegetarian,page));
        }
        #endregion

        #region Get information about concrete dish
        /// <summary>
        /// Get information about concrete dish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<DishDto>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        public async Task<IActionResult> GetDishDetails(Guid id)
        {
            return Ok(await _dishRepository.GetDishDetails(id)); 
        }
        #endregion

        #region Checks if user is able to set rating of the dish
        /// <summary>
        /// Checks if user is able to set rating of the dish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/rating/check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CheckRating(Guid id)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            return Ok(await _dishRepository.CheckRating(id,userId));
        }
        #endregion

        #region Set a rating for a dish
        /// <summary>
        ///  Set a rating for a dish
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ratingScore"></param>
        /// <returns></returns>
        [HttpPost("{id}/rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(Response), 500)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SetRating(Guid id,int ratingScore)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _dishRepository.SetRating(id, ratingScore, userId);
            return Ok();
        }
        #endregion

    }
}

