using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CleanMvcApp.Services;

namespace CleanMvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<AdminReviewController> _logger;

        public AdminReviewController(
            IReviewService reviewService,
            ILogger<AdminReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        // POST: AdminReview/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string returnUrl)
        {
            try
            {
                var result = await _reviewService.DeleteReviewAsync(id);
                if (result)
                {
                    TempData["Success"] = "Review deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Review not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", id);
                TempData["Error"] = "An error occurred while deleting the review.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}
