using Bogus;
using Bogus.Extensions;
using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbContext;
        public RestaurantSeeder(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if(_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }

            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Manager"
                },
                new Role()
                {
                    Name = "Admin"
                },
            };

            return roles;
        }

        #region Adding of Restaurants on my own
        //private IEnumerable<Restaurant> GetRestaurants()
        //{ 
        //    var restaurants = new List<Restaurant>()
        //    {
        //        new Restaurant()
        //        {
        //            Name = "KFC",
        //            Category = "Fast Food",
        //            Description =
        //                "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant chain headquartered in Louisville, Kentucky, that specializes in fried chicken.",
        //            ContactEmail = "contact@kfc.com",
        //            HasDelivery = true,
        //            Dishes = new List<Dish>()
        //            {
        //                new Dish()
        //                {
        //                    Name = "Nashville Hot Chicken",
        //                    Description = "Fresh meat with spice sauce",
        //                    Price = 10.30M,
        //                },

        //                new Dish()
        //                {
        //                    Name = "Chicken Nuggets",
        //                    Description = "With salad and chips",
        //                    Price = 5.30M,
        //                },
        //            },
        //            Address = new Address()
        //            {
        //                City = "Kraków",
        //                Street = "Długa 5",
        //                PostalCode = "30-001"
        //            }
        //        },
        //        new Restaurant()
        //        {
        //            Name = "McDonald Szewska",
        //            Category = "Fast Food",
        //            Description =
        //                "McDonald's Corporation (McDonald's), incorporated on December 21, 1964, operates and franchises McDonald's restaurants.",
        //            ContactEmail = "contact@mcdonald.com",
        //            HasDelivery = true,
        //            Address = new Address()
        //            {
        //                City = "Kraków",
        //                Street = "Szewska 2",
        //                PostalCode = "30-001"
        //            }
        //        }
        //    };

        //    return restaurants;
        //}
        #endregion
        private IEnumerable<Restaurant> GetRestaurants()
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
                .RuleFor(r => r.Dishes, f => f.Random.ListItems(dishes, 5).Select(d => d.ShallowCopy()).ToList());

            var restaurants = restaurantGenerator.Generate(100);

            return restaurants;

            //_dbContext.AddRange(restaurants);

            //_dbContext.SaveChanges();
        }

        private List<Dish> GetRandomRestaurants()
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
