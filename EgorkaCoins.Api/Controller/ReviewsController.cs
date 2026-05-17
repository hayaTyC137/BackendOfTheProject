using EgorkaCoins.Api.Filters;
using EgorkaCoins.BusinessLogic.Core;
using EgorkaCoins.Helpers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EgorkaCoins.Api.Controller
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewActions _reviewActions = new ReviewActions();

        [HttpGet]
        public IActionResult GetAll()
        {
            var reviews = _reviewActions.GetAll();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var review = _reviewActions.GetById(id);
            if (review == null)
                return NotFound(new { message = $"Review {id} not found" });
            return Ok(review);
        }

        [HttpGet("my")]
        [RequireAuth]
        public IActionResult GetMy()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var reviews = _reviewActions.GetByUserId(userId.Value);
            return Ok(reviews);
        }

        [HttpPost]
        [RequireAuth]
        public IActionResult Create([FromBody] CreateReviewRequest request)
        {
            var validation = ValidateReviewRequest(request);
            if (validation != null) return validation;

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var (result, review) = _reviewActions.CreateForUser(request, userId.Value);
            if (result == ReviewActionResult.Forbidden)
                return ForbiddenResponse();

            return StatusCode(201, review);
        }

        [HttpPut("{id}")]
        [RequireAuth]
        public IActionResult Update(int id, [FromBody] CreateReviewRequest request)
        {
            var validation = ValidateReviewRequest(request);
            if (validation != null) return validation;

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var (result, review) = _reviewActions.UpdateForUser(id, request, userId.Value);
            if (result == ReviewActionResult.NotFound)
                return NotFound(new { message = $"Review {id} not found" });
            if (result == ReviewActionResult.Forbidden)
                return ForbiddenResponse();

            return Ok(review);
        }

        [HttpDelete("{id}")]
        [RequireAuth]
        public IActionResult Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var result = _reviewActions.DeleteAllowed(id, userId.Value);
            if (result == ReviewActionResult.NotFound)
                return NotFound(new { message = $"Review {id} not found" });
            if (result == ReviewActionResult.Forbidden)
                return ForbiddenResponse();

            return NoContent();
        }

        [HttpDelete("my/{id}")]
        [RequireAuth]
        public IActionResult DeleteMy(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Не авторизован" });

            var result = _reviewActions.DeleteForUser(id, userId.Value);
            if (result == ReviewActionResult.NotFound)
                return NotFound(new { message = $"Review {id} not found" });

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("userId");
        }

        private IActionResult ForbiddenResponse()
        {
            return StatusCode(403, new { message = "Недостаточно прав, требуется роль admin или moderator" });
        }

        private IActionResult? ValidateReviewRequest(CreateReviewRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Game) ||
                string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { message = "Заполните все поля отзыва" });
            }

            if (request.Stars < 1 || request.Stars > 5)
                return BadRequest(new { message = "Оценка должна быть от 1 до 5" });

            return null;
        }
    }
}
