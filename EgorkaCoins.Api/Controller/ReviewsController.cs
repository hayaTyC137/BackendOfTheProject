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

        // GET api/reviews
        [HttpGet]
        public IActionResult GetAll()
        {
            var reviews = _reviewActions.GetAll();
            return Ok(reviews);
        }

        // GET api/reviews/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var review = _reviewActions.GetById(id);
            if (review == null)
                return NotFound(new { message = $"Review {id} not found" });
            return Ok(review);
        }

        // POST api/reviews
        [HttpPost]
        [RequireAuth]
        [AdminMod]
        public IActionResult Create([FromBody] CreateReviewRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) ||
                string.IsNullOrEmpty(request.Game) ||
                string.IsNullOrEmpty(request.Text))
            {
                return BadRequest(new { message = "Заполните все поля отзыва" });
            }

            if (request.Stars < 1 || request.Stars > 5)
                return BadRequest(new { message = "Оценка должна быть от 1 до 5" });

            var review = _reviewActions.Create(request);
            return StatusCode(201, review);
        }

        // PUT api/reviews/5
        [HttpPut("{id}")]
        [RequireAuth]
        [AdminMod]
        public IActionResult Update(int id, [FromBody] UpdateReviewRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) ||
                string.IsNullOrEmpty(request.Game) ||
                string.IsNullOrEmpty(request.Text))
            {
                return BadRequest(new { message = "Заполните все поля отзыва" });
            }

            if (request.Stars < 1 || request.Stars > 5)
                return BadRequest(new { message = "Оценка должна быть от 1 до 5" });

            var review = _reviewActions.Update(id, request);
            if (review == null)
                return NotFound(new { message = $"Review {id} not found" });
            return Ok(review);
        }

        // DELETE api/reviews/5
        [HttpDelete("{id}")]
        [RequireAuth]
        [AdminMod]
        public IActionResult Delete(int id)
        {
            var result = _reviewActions.Delete(id);
            if (!result)
                return NotFound(new { message = $"Review {id} not found" });
            return NoContent();
        }
    }
}
