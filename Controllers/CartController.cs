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



        [HttpGet]
        public IActionResult Checkout()
        {
            var cartIds = HttpContext.Session.Get<List<int>>("Cart") ?? new List<int>();
            if (!cartIds.Any())
            {
                return RedirectToAction("Index");
            }

            var cartDishes = _context.Dishes.Where(d => cartIds.Contains(d.Id)).ToList();
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

            var cartDishes = _context.Dishes.Where(d => cartIds.Contains(d.Id)).ToList();

            var order = new Order
            {
                OrderDate = DateTime.Now,
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

        public IActionResult Success()
        {
            return View();
        }
    }
}