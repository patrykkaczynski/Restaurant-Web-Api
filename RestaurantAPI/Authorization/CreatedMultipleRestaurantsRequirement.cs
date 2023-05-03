using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class CreatedMultipleRestaurantsRequirement : IAuthorizationRequirement
    {
        public int NumberOfCreatedRestaurants { get; }
        public CreatedMultipleRestaurantsRequirement(int numberOfCreatedRestaurants)
        {
            NumberOfCreatedRestaurants = numberOfCreatedRestaurants;
        }
    }
}
