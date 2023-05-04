using Bogus;
using Bogus.Extensions;
using Microsoft.AspNetCore.Session;
using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class DataGenerator
    {
        public static void Seed(RestaurantDbContext context)
        {
            Randomizer.Seed = new Random(911);

            var addressGenerator = new Faker<Address>()
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
                .RuleFor(a => a.Street, f => f.Address.StreetName());

            var dishes = GetRandomRestaurants();

            var restaurantGenerator = new Faker<Restaurant>()
                .RuleFor(r => r.Name, f => f.Company.CompanyName().ClampLength(3, 25))
                .RuleFor(r => r.Description, f => f.Company.CatchPhrase())
                .RuleFor(r => r.Category, f => f.Commerce.Categories(1).First())
                .RuleFor(r => r.HasDelivery, f => f.Random.Bool())
                .RuleFor(r => r.ContactEmail, f => f.Internet.Email())
                .RuleFor(r => r.ContactNumber, f => f.Phone.PhoneNumber("###-###-###"))
                .RuleFor(r => r.CreatedById, f => f.Random.Int(1, 20))
                .RuleFor(r => r.Address, f => addressGenerator.Generate())
                //.RuleFor(r => r.Dishes, f => dishes.OrderBy(x => Random.Shared.Next()).Take(5).ToList());
                .RuleFor(r => r.Dishes, f => f.Random.ListItems(dishes, 5));
        
            var restaurants = restaurantGenerator.Generate(100);

            context.AddRange(restaurants);

            context.SaveChanges();

        }

        private static List<Dish> GetRandomRestaurants()
        {
            List<Dish> dishes = new List<Dish>()
            {
                new Dish()
                {
                    Name = "Pizza Margerita",
                    Description = "Tomatoes, sliced mozzarella, basil, and extra virgin olive oil",
                    Price = 12.30M
                },
                new Dish()
                {
                    Name = "Pizza Marinara",
                    Description = "Tomatoes, garlic, oregano, and extra virgin olive oil",
                    Price = 13.10M
                },
                new Dish()
                {
                    Name = "Pizza Sicilian",
                    Description = "Tomatoes, onion, anchovies, and herbs",
                    Price = 16.50M
                },
                new Dish()
                {
                    Name = "Meat Lover's Pizza",
                    Description = "Two types of cheese, bacon, ham, pepperoni and hot sausage",
                    Price = 18.90M
                },
                new Dish()
                {
                    Name = "Beet and goat cheese salad",
                    Description = "Beet, goat cheese, peppery arugula, crunchy candied pecans, and tangy balsamic vinaigrette",
                    Price = 6.90M
                },
                new Dish()
                {
                    Name = "Cobb salad",
                    Description = "Avocado, hard-boiled egg, chicken, bacon and red-wine vinegar dressing",
                    Price = 9.20M
                },
                new Dish()
                {
                    Name = "Caesar salad",
                    Description = "Raw eggs and anchovies in traditional Caesar dressing",
                    Price = 7.10M
                },
                new Dish()
                {
                    Name = "Bolognese Lasagna",
                    Description = "Lasagna noodles, sausage, beef, and bacon",
                    Price = 14.70M
                },
                new Dish()
                {
                    Name = "Fettuccine alla Carbonara",
                    Description = "Butter, parmesan, white wine, and whipping cream",
                    Price = 12.90M
                },
            };

            return dishes;
        }
    }
}
