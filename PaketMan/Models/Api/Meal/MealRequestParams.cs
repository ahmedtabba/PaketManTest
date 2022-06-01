using Newtonsoft.Json;

namespace PaketMan.Models.Api.Meal
{
    public class MealRequestParams : PaginationFilter
    {
        [JsonProperty("searchText")]
        public string? SearchText { get; set; }
        [JsonProperty("restaurantId")]
        public int? RestaurantId { get; set; }
        [JsonProperty("sort")]
        public string? Sort { get; set; }

        public MealRequestParams() : base()
        {
        }
        public MealRequestParams(int pageNumber, int pageSize) : base(pageNumber, pageSize)
        {
        }
    }
}
