using Microsoft.AspNetCore.Mvc;
using ItalianRestaurant.Models;
using Microsoft.EntityFrameworkCore;
using ItalianRestaurant.Data;
using System.Security.Claims;

namespace ItalianRestaurant.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cartIds = HttpContext.Session.Get<List<int>>("Cart") ?? new List<int>();
            var cartDishes = await _context.Dishes.Where(d => cartIds.Contains(d.Id)).ToListAsync();

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

            return RedirectToAction("Index", "Menu");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cartIds = HttpContext.Session.Get<List<int>>("Cart") ?? new List<int>();
            if (!cartIds.Any())
            {
                return RedirectToAction("Index");
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        ViewBag.DefaultName = user.FullName;
                        ViewBag.DefaultPhone = user.Phone;
                    }
                }
            }

            var cartDishes = await _context.Dishes.Where(d => cartIds.Contains(d.Id)).ToListAsync();
            return View(cartDishes);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(
            string customerName,
            string deliveryAddress,
            string phone,
            string paymentMethod,
            string deliveryTime,
            string? comment)
        {
            var cartIds = HttpContext.Session.Get<List<int>>("Cart") ?? new List<int>();
            if (!cartIds.Any())
            {
                return RedirectToAction("Index");
            }

            int? currentUserId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int parsedId))
                {
                    currentUserId = parsedId;
                }
            }

            var cartDishes = await _context.Dishes.Where(d => cartIds.Contains(d.Id)).ToListAsync();

            var order = new Order
            {
                UserId = currentUserId,
                OrderDate = DateTime.UtcNow,
                CustomerName = customerName,
                DeliveryAddress = deliveryAddress,
                Phone = phone,
                PaymentMethod = paymentMethod,
                DeliveryTime = deliveryTime,
                Comment = comment,
                Status = "New",
                TotalAmount = cartDishes.Sum(d => d.Price)
            };
            foreach (var dish in cartDishes)
            {
                order.OrderItems.Add(new OrderItem
                {
                    DishId = dish.Id,
                    PriceAtMoment = dish.Price,
                    Quantity = 1
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success", new { id = order.Id });
        }

        public IActionResult ProceedToCheckout()
        {
            var cartIds = HttpContext.Session.Get<List<int>>("Cart") ?? new List<int>();
            if (!cartIds.Any())
            {
                return RedirectToAction("Index");
            }

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ReturnUrl"] = Url.Action("Checkout", "Cart");
                TempData["AuthError"] = "Please sign in or register to continue checkout and save your order history.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Checkout");
        }

        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}