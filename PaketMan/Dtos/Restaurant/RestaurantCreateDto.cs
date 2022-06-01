using System.ComponentModel.DataAnnotations;

namespace PaketMan.Dtos.Restaurant
{
    public class RestaurantCreateDto
    {
        [Required(AllowEmptyStrings =false)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
    }
}
