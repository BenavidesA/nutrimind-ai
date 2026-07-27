using NutriMind.Domain.Common;
using System;

namespace NutriMind.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public new DateTime CreatedAt { get; set; }
        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
