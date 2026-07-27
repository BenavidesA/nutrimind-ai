using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class UserSettings : BaseEntity
    {
        public string Theme { get; set; } = "Light";
        public string Language { get; set; } = "en-US";
        public bool NotificationsEnabled { get; set; } = true;

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
