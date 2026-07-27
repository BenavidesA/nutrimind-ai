using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class WeightRecord : BaseEntity
    {
        public DateTime Date { get; set; }
        public decimal WeightKg { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
