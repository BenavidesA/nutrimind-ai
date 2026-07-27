using NutriMind.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace NutriMind.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // User Module
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        // Food Module
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FavoriteFood> FavoriteFoods { get; set; }

        // Records Module
        public DbSet<MealType> MealTypes { get; set; }
        public DbSet<FoodLog> FoodLogs { get; set; }
        public DbSet<DailyIntakeSummary> DailyIntakeSummaries { get; set; }

        // Plans Module
        public DbSet<NutritionGoal> NutritionGoals { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<PlannedMeal> PlannedMeals { get; set; }
        public DbSet<WeightRecord> WeightRecords { get; set; }

        // AI Module
        public DbSet<AIConversation> AIConversations { get; set; }
        public DbSet<AIMessage> AIMessages { get; set; }
        public DbSet<AIRecommendation> AIRecommendations { get; set; }

        // Gamification Module
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }
        public DbSet<PointsLedger> PointsLedgers { get; set; }
        public DbSet<Streak> Streaks { get; set; }

        // Maps Module
        public DbSet<RestaurantCache> RestaurantCaches { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
