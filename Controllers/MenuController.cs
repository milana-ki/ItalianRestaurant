using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using ItalianRestaurant.Data;

namespace ItalianRestaurant.Controllers
{
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dishes = _context.Dishes
                .Include(d => d.Category)
                .ToList();

            return View(dishes);
        }
    }
}