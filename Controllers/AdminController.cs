using System.Security.Claims;
using ItalianRestaurant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItalianRestaurant.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private const string AdminEmail = "kilbovych.milana@gmail.com";

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsCurrentUserAdmin()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return false;
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            return string.Equals(email, AdminEmail, StringComparison.OrdinalIgnoreCase);
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            if (!IsCurrentUserAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            if (!IsCurrentUserAdmin())
            {
                return RedirectToAction("Index", "Home");
            }

            var allowedStatuses = new[] { "New", "Cooking", "Delivering", "Completed", "Cancelled" };
            if (!allowedStatuses.Contains(status))
            {
                TempData["AdminError"] = "Invalid order status.";
                return RedirectToAction(nameof(Orders));
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                TempData["AdminError"] = $"Order #{orderId} not found.";
                return RedirectToAction(nameof(Orders));
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = $"Order #{order.Id} status updated to \"{status}\".";
            return RedirectToAction(nameof(Orders));
        }
    }
}