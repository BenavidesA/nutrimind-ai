using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public new DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
