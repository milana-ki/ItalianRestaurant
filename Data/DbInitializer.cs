using ItalianRestaurant.Models;
using System.Linq;

namespace ItalianRestaurant.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Dishes.Any())
            {
                return;
            }

            if (!context.Categories.Any())
            {
                var categories = new Category[]
                {
                    new Category { Name = "Antipasti", Description = "Traditional Italian starters and appetizers." },
                    new Category { Name = "Pasta", Description = "Classic handmade Italian pasta dishes." },
                    new Category { Name = "Pizza", Description = "Authentic Neapolitan wood-fired pizzas." },
                    new Category { Name = "Desserts", Description = "Sweet Italian treats and traditional desserts." },
                    new Category { Name = "Drinks", Description = "Refreshing beverages, coffee, and drinks." },
                    new Category { Name = "Soups", Description = "Hearty Italian soups and creamy bisques." },
                    new Category { Name = "Side Dishes", Description = "Delicious roasted and grilled accompaniments." },
                    new Category { Name = "Salads", Description = "Fresh Mediterranean salads with rich dressings." }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
            var catAntipasti = context.Categories.First(c => c.Name == "Antipasti").Id;
            var catPasta = context.Categories.First(c => c.Name == "Pasta").Id;
            var catPizza = context.Categories.First(c => c.Name == "Pizza").Id;
            var catDesserts = context.Categories.First(c => c.Name == "Desserts").Id;
            var catDrinks = context.Categories.First(c => c.Name == "Drinks").Id;
            var catSoups = context.Categories.First(c => c.Name == "Soups").Id;
            var catSideDishes = context.Categories.First(c => c.Name == "Side Dishes").Id;
            var catSalads = context.Categories.First(c => c.Name == "Salads").Id;

            var dishes = new Dish[]
            {
                // Antipasti
                new Dish { Name = "Tomato Bruschetta", Price = 185, Description = "Toasted ciabatta with juicy tomatoes, fresh basil, garlic, and olive oil.", CategoryId = catAntipasti, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Caprese Salad", Price = 265, Description = "Ripe tomatoes, fresh mozzarella, basil, and olive oil.", CategoryId = catAntipasti, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Antipasto Platter", Price = 345, Description = "Selection of Italian cured meats, cheeses, olives, and breadsticks.", CategoryId = catAntipasti, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Fried Mozzarella", Price = 235, Description = "Crispy breaded mozzarella served with homemade tomato sauce.", CategoryId = catAntipasti, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Rosemary Focaccia", Price = 145, Description = "Traditional flatbread baked with olive oil, sea salt, and rosemary.", CategoryId = catAntipasti, ImageUrl = "/images/default.jpg" },

                // Pasta
                new Dish { Name = "Spaghetti Carbonara", Price = 295, Description = "Classic Roman pasta with guanciale, Pecorino Romano, egg yolks, and black pepper.", CategoryId = catPasta, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Spaghetti Bolognese", Price = 285, Description = "Rich beef and tomato ragù slowly simmered with vegetables and Parmesan.", CategoryId = catPasta, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Fettuccine Alfredo", Price = 295, Description = "Fettuccine in a smooth cream sauce with Parmesan and butter.", CategoryId = catPasta, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Penne Arrabbiata", Price = 255, Description = "Spicy tomato sauce with garlic, chili peppers, olive oil, and parsley.", CategoryId = catPasta, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Seafood Spaghetti", Price = 365, Description = "Spaghetti with shrimp, mussels, and squid in a savory garlic tomato sauce.", CategoryId = catPasta, ImageUrl = "/images/default.jpg" },

                // Pizza
                new Dish { Name = "Margherita", Price = 285, Description = "Classic pizza with tomato sauce, mozzarella, and fresh basil.", CategoryId = catPizza, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Pepperoni", Price = 335, Description = "Tomato sauce, melted mozzarella, and spicy pepperoni slices.", CategoryId = catPizza, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Four Cheese", Price = 365, Description = "Rich combination of mozzarella, gorgonzola, parmesan, and ricotta.", CategoryId = catPizza, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Prosciutto & Arugula", Price = 385, Description = "Tomato sauce, mozzarella, delicate prosciutto, fresh arugula, and parmesan shavings.", CategoryId = catPizza, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Capricciosa", Price = 355, Description = "Tomato sauce, mozzarella, ham, mushrooms, artichokes, and olives.", CategoryId = catPizza, ImageUrl = "/images/default.jpg" },

                // Desserts
                new Dish { Name = "Tiramisu", Price = 195, Description = "Classic dessert with mascarpone cheese, espresso-soaked ladyfingers, and cocoa.", CategoryId = catDesserts, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Panna Cotta", Price = 175, Description = "Delicate vanilla cream dessert served with berry sauce.", CategoryId = catDesserts, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Cannoli", Price = 185, Description = "Crispy pastry shells filled with sweet ricotta, pistachios, and powdered sugar.", CategoryId = catDesserts, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Chocolate Fondant", Price = 205, Description = "Warm chocolate cake with a molten center, served with vanilla ice cream.", CategoryId = catDesserts, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Gelato", Price = 145, Description = "Traditional Italian ice cream. Choice of vanilla, pistachio, or chocolate.", CategoryId = catDesserts, ImageUrl = "/images/default.jpg" },

                // Drinks
                new Dish { Name = "Espresso", Price = 65, Description = "Rich and bold traditional Italian shot of coffee.", CategoryId = catDrinks, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Cappuccino", Price = 95, Description = "Espresso topped with steamed milk and dense milk foam.", CategoryId = catDrinks, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Homemade Lemonade", Price = 115, Description = "Refreshing lemon juice with sparkling water, fresh mint, and ice.", CategoryId = catDrinks, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Orange Fresh", Price = 125, Description = "100% freshly squeezed natural orange juice.", CategoryId = catDrinks, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Mineral Water", Price = 75, Description = "Chilled lightly sparkling mineral water.", CategoryId = catDrinks, ImageUrl = "/images/default.jpg" },

                // Soups
                new Dish { Name = "Minestrone", Price = 175, Description = "Traditional vegetable soup with beans, seasonal greens, and small pasta.", CategoryId = catSoups, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Tomato Cream Soup", Price = 165, Description = "Creamy ripe tomato soup seasoned with garlic and fresh herbs, served with croutons.", CategoryId = catSoups, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Mushroom Soup", Price = 185, Description = "Rich soup made from wild mushrooms and cream.", CategoryId = catSoups, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Zuppa di Pesce", Price = 295, Description = "Seafood soup with shrimp, mussels, squid, and white fish in garlic tomato broth.", CategoryId = catSoups, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Tortellini in Brodo", Price = 195, Description = "Stuffed mini tortellini served in clear chicken broth with parmesan.", CategoryId = catSoups, ImageUrl = "/images/default.jpg" },

                // Side Dishes
                new Dish { Name = "Rustic Baked Potatoes", Price = 115, Description = "Crispy potato wedges baked with rosemary, garlic, and olive oil.", CategoryId = catSideDishes, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Grilled Vegetables", Price = 145, Description = "Assorted zucchini, eggplant, and bell peppers grilled with Italian herbs.", CategoryId = catSideDishes, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Mashed Potatoes", Price = 95, Description = "Creamy whipped mashed potatoes prepared with fresh butter and milk.", CategoryId = catSideDishes, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Green Beans", Price = 125, Description = "Sautéed green beans with garlic, olive oil, and fresh lemon juice.", CategoryId = catSideDishes, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Parmesan Polenta", Price = 135, Description = "Creamy cornmeal side dish served with butter and aged Parmesan cheese.", CategoryId = catSideDishes, ImageUrl = "/images/default.jpg" },

                // Salads
                new Dish { Name = "Chicken Caesar Salad", Price = 265, Description = "Romaine lettuce, grilled chicken breast, croutons, parmesan, and Caesar dressing.", CategoryId = catSalads, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Panzanella Salad", Price = 225, Description = "Tuscan salad with tomatoes, cucumbers, soaked bread, red onions, and fresh basil.", CategoryId = catSalads, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Pear & Gorgonzola Salad", Price = 285, Description = "Fresh arugula, sweet pears, gorgonzola cheese, walnuts, and honey-olive dressing.", CategoryId = catSalads, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Seafood Salad", Price = 345, Description = "Shrimp, squid, and mussels on fresh greens with cherry tomatoes and lemon dressing.", CategoryId = catSalads, ImageUrl = "/images/default.jpg" },
                new Dish { Name = "Tuna Salad", Price = 295, Description = "Tuna chunks, tomatoes, red onions, fresh lettuce leaves, olives, and lemon dressing.", CategoryId = catSalads, ImageUrl = "/images/default.jpg" }
            };

            context.Dishes.AddRange(dishes);
            context.SaveChanges();
        }
    }
}