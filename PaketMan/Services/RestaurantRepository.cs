using Dapper;
using PaketMan.Context;
using PaketMan.Contracts;
using PaketMan.Dtos.Restaurant;
using PaketMan.Models;
using PaketMan.Utallites;
using System.Data;

namespace PaketMan.Services
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly DapperContext _context;
        public RestaurantRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Restaurant> Create(RestaurantCreateDto restaurant)
        {

            var query = $"INSERT INTO \"{Consts.Restaurants}\" (\"Name\", \"City\", \"Description\",\"OwnerId\") VALUES (@Name, @City, @Description,@OwnerId) RETURNING \"Id\";";
            var parameters = new DynamicParameters();
            parameters.Add("Name", restaurant.Name, DbType.String);
            parameters.Add("City", restaurant.City, DbType.String);
            parameters.Add("Description", restaurant.Description, DbType.String);
            parameters.Add("OwnerId", restaurant.OwnerId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdRestaurant = new Restaurant
                {
                    Id = id,
                    Name = restaurant.Name,
                    City = restaurant.City,
                    Description = restaurant.Description,
                    OwnerId = restaurant.OwnerId
                };
                return createdRestaurant;
            }

        }

        public async Task Delete(int id)
        {
            var query = $"DELETE FROM \"{Consts.Restaurants}\" WHERE \"Id\" = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<IQueryable<Restaurant>> GetAll()
        {
            try
            {
                var query = $"SELECT * FROM \"{Consts.Restaurants}\";";
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var restaurants = await connection.QueryAsync<Restaurant>(query);
                    return restaurants.AsQueryable();
                }
            }
            catch
            {

                throw;
            }
        }

        public async Task<IQueryable<Restaurant>> GetAllWithMealsPricesSumOverThousand()
        {
            try
            {
                var query = @"SELECT * FROM  ""Restaurants""
                	Where ""Id"" in (Select ""RestaurantId"" From ""Meals"" 
				   GROUP BY ""RestaurantId""
                   HAVING Sum(""Price"")>1000)";
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var restaurants = await connection.QueryAsync<Restaurant>(query);
                    return restaurants.AsQueryable();
                }
            }
            catch
            {

                throw;
            }
        }

        public async Task<Restaurant> GetByIdAsync(int id)
        {
            try
            {
                var query = $"SELECT * FROM \"{Consts.Restaurants}\" WHERE \"Id\" = @Id";
                using (var connection = _context.CreateConnection())
                {
                    var restaurant = await connection.QuerySingleOrDefaultAsync<Restaurant>(query, new { id });
                    return restaurant;
                }
            }
            catch
            {

                throw;
            }
        }

        public async Task<IQueryable<Restaurant>> GetRestaurantsWithDetails()
        {
            var query = $"SELECT * FROM \"{Consts.Restaurants}\" FULL OUTER JOIN \"{Consts.Meals}\"  ON \"{Consts.Restaurants}\".\"Id\" = \"{Consts.Meals}\".\"RestaurantId\";";
            using (var connection = _context.CreateConnection())
            {
                var restaurantDict = new Dictionary<int, Restaurant>();
                var restaurants = await connection.QueryAsync<Restaurant, Meal, Restaurant>(
                    query, (restaurant, meal) =>
                    {
                        if (!restaurantDict.TryGetValue(restaurant.Id, out var currentRestaurant))
                        {
                            currentRestaurant = restaurant;
                            restaurantDict.Add(currentRestaurant.Id, currentRestaurant);
                        }
                        if (meal != null)
                            currentRestaurant.Meals.Add(meal);
                        return currentRestaurant;
                    }
                );
                return restaurants.Distinct().AsQueryable();
            }
        }

        public async Task Update(int id, RestaurantUpdateDto restaurant)
        {
            var query = $"UPDATE \"{Consts.Restaurants}\" SET \"Name\" = @Name, \"City\" = @City, \"Description\" = @Description, \"OwnerId\" = @OwnerId WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Name", restaurant.Name, DbType.String);
            parameters.Add("City", restaurant.City, DbType.String);
            parameters.Add("Description", restaurant.Description, DbType.String);
            parameters.Add("OwnerId", restaurant.OwnerId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<string> IsValid(RestaurantCreateDto obDto)
        {
            var res = "";



            res = await CheckRestaurantOwnerValidation(obDto.OwnerId);
            return Task.FromResult(res).Result;
        }

        public async Task<string> IsValid(int id, RestaurantUpdateDto obDto)
        {
            string res = "";

            res = await CheckRestaurantOwnerValidation(obDto.OwnerId, id);

            return Task.FromResult(res).Result;
        }

        private async Task<string> CheckRestaurantOwnerValidation(int OwnerId, int? id = null)
        {
            string res = "";
            var query = "";
            Restaurant restaurant = null;
            using (var connection = _context.CreateConnection())
            {
                if (id.HasValue)
                {
                    query = $"SELECT * FROM \"{Consts.Restaurants}\" WHERE \"OwnerId\" = @OwnerId And \"Id\"!=@Id";
                    restaurant = await connection.QuerySingleOrDefaultAsync<Restaurant>(query, new { OwnerId, id });
                }
                else
                {
                    query = $"SELECT * FROM \"{Consts.Restaurants}\" WHERE \"OwnerId\" = @OwnerId";
                    restaurant = await connection.QuerySingleOrDefaultAsync<Restaurant>(query, new { OwnerId });
                }


                if (restaurant != null)
                    res = "This User is already linked with another restaurant!!";
            }

            return res;
        }
    }
}
