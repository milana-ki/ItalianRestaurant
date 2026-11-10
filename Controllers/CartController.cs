using Microsoft.AspNetCore.Mvc;
using ItalianRestaurant.Models;
using Microsoft.EntityFrameworkCore;
using ItalianRestaurant.Data;

namespace ItalianRestaurant.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cartIds = HttpContext.Session.Get<List<int>>("Cart") ?? new List<int>();

            var cartDishes = _context.Dishes.Where(d => cartIds.Contains(d.Id)).ToList();

            return View(cartDishes);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int dishId)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.Get<List<int>>("Cart") ?? new List<int>();

            if (cart.Contains(dishId))
            {
                cart.Remove(dishId);
            }
            else
            {
                cart.Add(dishId);
            }

            HttpContext.Session.Set("Cart", cart);

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Menu", "Home");
        }
    }
}