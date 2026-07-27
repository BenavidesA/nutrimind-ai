using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class DailyIntakeSummary : BaseEntity
    {
        public DateTime SummaryDate { get; set; }
        public decimal TotalCalories { get; set; }
        public decimal TotalProtein { get; set; }
        public decimal TotalCarbs { get; set; }
        public decimal TotalFat { get; set; }
        public decimal TotalFiber { get; set; }
        public int WaterMl { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
