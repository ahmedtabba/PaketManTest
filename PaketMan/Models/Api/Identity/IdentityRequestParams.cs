using Newtonsoft.Json;

namespace PaketMan.Models.Api.Identity
{
    public class IdentityRequestParams : PaginationFilter
    {
        [JsonProperty("searchText")]
        public string? SearchText { get; set; }
       
        [JsonProperty("sort")]
        public string? Sort { get; set; }

        public IdentityRequestParams() : base()
        {
        }
        public IdentityRequestParams(int pageNumber, int pageSize) : base(pageNumber, pageSize)
        {
        }
    }
}
