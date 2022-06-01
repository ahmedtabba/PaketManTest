using Newtonsoft.Json;

namespace PaketMan.Models.Api.Restaurant
{
    public class RestaurantRequestParams : PaginationFilter
    {
        [JsonProperty("searchText")]
        public string? SearchText { get; set; }
        [JsonProperty("sort")]
        public string? Sort { get; set; }

        public RestaurantRequestParams() : base()
        {
        }
        public RestaurantRequestParams(int pageNumber, int pageSize) : base(pageNumber, pageSize)
        {
        }
    }
}
