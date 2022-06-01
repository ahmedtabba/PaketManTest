using PaketMan.Dtos.Meal;
using PaketMan.Models;

namespace PaketMan.Contracts
{
    public interface IMealRepository
    {
        public Task<IQueryable<Meal>> GetAll();
        public Task<IQueryable<Meal>> GetByRestaurantId(int restaurantId);
        public Task<Meal> GetById(int id);
        public Task<Meal> Create(MealCreateDto meal);
        public Task Update(int id, MealUpdateDto meal);
        public Task Delete(int id);
    }
}
