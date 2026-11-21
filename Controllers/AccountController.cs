using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItalianRestaurant.Data;
using ItalianRestaurant.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ItalianRestaurant.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Profile()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["AuthError"] = "Please log in to view your profile.";
                return RedirectToAction("Index", "Home");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string FullName, string Email, string Phone)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var currentEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentEmail);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (Email != currentEmail && await _context.Users.AnyAsync(u => u.Email == Email))
            {
                TempData["ProfileError"] = "This email is already taken by another account.";
                return RedirectToAction("Profile");
            }

            user.FullName = FullName;
            user.Email = Email;
            user.Phone = Phone;

            await _context.SaveChangesAsync();

            await SignInUser(user);

            TempData["ProfileSuccess"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                var userOrders = await _context.Orders.Where(o => o.UserId == user.Id).ToListAsync();
                foreach (var order in userOrders)
                {
                    order.UserId = null;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string phone, string? returnUrl = null)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                TempData["AuthError"] = "User with this email already exists.";
                TempData["ActiveTab"] = "register";
                return RedirectToRefererOrHome();
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Client"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SignInUser(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRefererOrHome();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                TempData["AuthError"] = "Invalid email or password.";
                TempData["ActiveTab"] = "login";
                return RedirectToRefererOrHome();
            }

            await SignInUser(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToRefererOrHome();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["ReturnUrl"] = Url.Action("MyOrders", "Account");
                TempData["AuthError"] = "Please log in to view your orders.";
                return RedirectToAction("Index", "Home");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Dish)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        private IActionResult RedirectToRefererOrHome()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}