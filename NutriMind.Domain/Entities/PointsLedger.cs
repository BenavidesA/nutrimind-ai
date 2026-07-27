using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class PointsLedger : BaseEntity
    {
        public int Points { get; set; }
        public string Reason { get; set; } // e.g., "Completed challenge", "Daily login"
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
