namespace RestaurantAPI.Entities
{
    public class Dish
    {
        public Dish ShallowCopy()
        {
            return (Dish)this.MemberwiseClone();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
    }
}
