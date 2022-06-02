using PaketMan.Dtos.Restaurant;
using PaketMan.Models;

namespace PaketMan.Contracts
{
    public interface IRestaurantRepository
    {
        public Task<IQueryable<Restaurant>> GetAll();
        public  Task<IQueryable<Restaurant>> GetRestaurantsWithDetails();
        public Task<Restaurant> GetByIdAsync(int id);
        public Task<Restaurant> Create(RestaurantCreateDto obDto);
        public Task Update(int id, RestaurantUpdateDto obDto);
        public Task Delete(int id);
        public Task<IQueryable<Restaurant>> GetAllWithMealsPricesSumOverThousand();
        public Task<string> IsValid(RestaurantCreateDto obDto);
        public Task<string> IsValid(int id,RestaurantUpdateDto obDto);

    }
}
