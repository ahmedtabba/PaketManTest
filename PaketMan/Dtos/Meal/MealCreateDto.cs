using System.ComponentModel.DataAnnotations;

namespace PaketMan.Dtos.Meal
{
    public class MealCreateDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Restaurant field is required")]
        public int RestaurantId { get; set; }
    }
}
