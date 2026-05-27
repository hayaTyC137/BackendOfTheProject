using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgorkaCoins.Api.Controllers.Community
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewActions _reviewActions = new ReviewActions();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll() => Ok(_reviewActions.GetAll());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var review = _reviewActions.GetById(id);
            if (review == null)
                return NotFound(new { message = $"Review {id} not found" });
            return Ok(review);
        }

        [HttpGet("my")]
        [Authorize]
        public IActionResult GetMy()
            => Ok(_reviewActions.GetByUserId(GetCurrentUserId()));

        [HttpPost]
        [Authorize]
        public IActionResult Create([FromBody] CreateReviewRequest request)
        {
            var validation = Validate(request);
            if (validation != null) return validation;

            var (result, review) = _reviewActions.CreateForUser(request, GetCurrentUserId());
            if (result == ReviewActionResult.Forbidden) return Forbidden();
            return StatusCode(201, review);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update(int id, [FromBody] CreateReviewRequest request)
        {
            var validation = Validate(request);
            if (validation != null) return validation;

            var (result, review) = _reviewActions.UpdateForUser(id, request, GetCurrentUserId());
            if (result == ReviewActionResult.NotFound)
                return NotFound(new { message = $"Review {id} not found" });
            if (result == ReviewActionResult.Forbidden) return Forbidden();
            return Ok(review);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var result = _reviewActions.DeleteAllowed(id, GetCurrentUserId());
            if (result == ReviewActionResult.NotFound)
                return NotFound(new { message = $"Review {id} not found" });
            if (result == ReviewActionResult.Forbidden) return Forbidden();
            return NoContent();
        }

        [HttpDelete("my/{id}")]
        [Authorize]
        public IActionResult DeleteMy(int id)
        {
            var result = _reviewActions.DeleteForUser(id, GetCurrentUserId());
            if (result == ReviewActionResult.NotFound)
                return NotFound(new { message = $"Review {id} not found" });
            return NoContent();
        }

        private int GetCurrentUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private IActionResult Forbidden()
            => StatusCode(403, new { message = "Недостаточно прав" });

        private IActionResult? Validate(CreateReviewRequest r)
        {
            if (string.IsNullOrWhiteSpace(r.Game) || string.IsNullOrWhiteSpace(r.Text))
                return BadRequest(new { message = "Заполните все поля отзыва" });
            if (r.Stars < 1 || r.Stars > 5)
                return BadRequest(new { message = "Оценка должна быть от 1 до 5" });
            return null;
        }
    }
}
