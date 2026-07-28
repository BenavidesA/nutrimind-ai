using NutriMind.Domain.Common;
using System;
using System.Collections.Generic;

namespace NutriMind.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }

        // --- Gamification ---
        public int CurrentStreak { get; set; } = 0;
        public int HighestStreak { get; set; } = 0;
        public DateTime? LastLogDate { get; set; }
        public int TotalPoints { get; set; } = 0;

        public virtual UserProfile UserProfile { get; set; }
        public virtual UserSettings UserSettings { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
        public virtual ICollection<FavoriteFood> FavoriteFoods { get; set; } = new List<FavoriteFood>();
        public virtual ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
        public virtual ICollection<DailyIntakeSummary> DailyIntakeSummaries { get; set; } = new List<DailyIntakeSummary>();
        public virtual ICollection<NutritionGoal> NutritionGoals { get; set; } = new List<NutritionGoal>();
        public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
        public virtual ICollection<WeightRecord> WeightRecords { get; set; } = new List<WeightRecord>();
        public virtual ICollection<AIConversation> AIConversations { get; set; } = new List<AIConversation>();
        public virtual ICollection<AIRecommendation> AIRecommendations { get; set; } = new List<AIRecommendation>();
        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
        public virtual ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();
        public virtual ICollection<PointsLedger> PointsLedgers { get; set; } = new List<PointsLedger>();
        public virtual ICollection<Streak> Streaks { get; set; } = new List<Streak>();
    }
}
