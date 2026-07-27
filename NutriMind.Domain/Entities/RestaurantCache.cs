using NutriMind.Domain.Common;

namespace NutriMind.Domain.Entities
{
    public class RestaurantCache : BaseEntity
    {
        public string GooglePlaceId { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double Rating { get; set; }
    }
}
