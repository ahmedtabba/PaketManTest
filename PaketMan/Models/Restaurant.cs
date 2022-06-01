namespace PaketMan.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Description { get; set; }


        public List<Meal> Meals { get; set; } = new List<Meal>();
    }
}
