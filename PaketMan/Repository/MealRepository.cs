using Dapper;
using PaketMan.Context;
using PaketMan.Contracts;
using PaketMan.Dtos.Meal;
using PaketMan.Models;
using PaketMan.Utallites;
using System.Data;

namespace PaketMan.Repository
{
    public class MealRepository : IMealRepository
    {
        private readonly DapperContext _context;
        public MealRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Meal> Create(MealCreateDto meal)
        {
            var query = $"INSERT INTO \"{Consts.Meals}\" (\"Name\", \"Price\", \"RestaurantId\") VALUES (@Name, @Price, @RestaurantId) RETURNING \"Id\";";
            var parameters = new DynamicParameters();
            parameters.Add("Name", meal.Name, DbType.String);
            parameters.Add("Price", meal.Price, DbType.Decimal);
            parameters.Add("RestaurantId", meal.RestaurantId);
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdMeal = new Meal
                {
                    Id = id,
                    Name = meal.Name,
                    Price = meal.Price,
                    RestaurantId = meal.RestaurantId
                };
                return createdMeal;
            }
        }

        public async Task Delete(int id)
        {
            var query = $"DELETE FROM \"{Consts.Meals}\" WHERE \"Id\" = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<IQueryable<Meal>> GetAll()
        {
            try
            {
                var query = $"SELECT * FROM \"{Consts.Meals}\";";
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var meals = await connection.QueryAsync<Meal>(query);
                    return meals.AsQueryable();
                }
            }
            catch
            {

                throw;
            }
        }

        public async Task<Meal> GetById(int id)
        {
            try
            {
                var query = $"SELECT * FROM \"{Consts.Meals}\" WHERE \"Id\" = @Id";
                using (var connection = _context.CreateConnection())
                {
                    var meal = await connection.QuerySingleOrDefaultAsync<Meal>(query, new { id });
                    return meal;
                }
            }
            catch
            {

                throw;
            }
        }

        public async Task Update(int id, MealUpdateDto meal)
        {

            var query = $"UPDATE \"{Consts.Meals}\" SET \"Name\" = @Name, \"Price\" = @Price, \"RestaurantId\" = @RestaurantId WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Name", meal.Name, DbType.String);
            parameters.Add("Price", meal.Price, DbType.Decimal);
            parameters.Add("RestaurantId", meal.RestaurantId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
