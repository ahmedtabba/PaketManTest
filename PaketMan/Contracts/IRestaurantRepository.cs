using PaketMan.Dtos.Restaurant;
using PaketMan.Models;

namespace PaketMan.Contracts
{
    public interface IRestaurantRepository
    {
        public Task<IQueryable<Restaurant>> GetAll();
        public  Task<IQueryable<Restaurant>> GetRestaurantsWithDetails();
        public Task<Restaurant> GetByIdAsync(int id);
        public Task<Restaurant> Create(RestaurantCreateDto restaurant);
        public Task Update(int id, RestaurantUpdateDto restaurant);
        public Task Delete(int id);
        public Task<IQueryable<Restaurant>> GetAllWithMealsPricesSumOverThousand();

    }
}
