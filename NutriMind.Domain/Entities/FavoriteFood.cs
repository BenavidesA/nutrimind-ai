using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class FavoriteFood : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid FoodId { get; set; }
        public virtual Food Food { get; set; }
    }
}
