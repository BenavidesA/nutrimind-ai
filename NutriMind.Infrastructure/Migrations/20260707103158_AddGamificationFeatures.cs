using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NutriMind.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGamificationFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedMeals_Foods_FoodId",
                table: "PlannedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedMeals_MealTypes_MealTypeId",
                table: "PlannedMeals");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteFoods_UserId",
                table: "FavoriteFoods");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "PasswordResetTokens");

            migrationBuilder.RenameColumn(
                name: "Revoked",
                table: "RefreshTokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "Expires",
                table: "RefreshTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "Expires",
                table: "PasswordResetTokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "DailyIntakeSummaries",
                newName: "SummaryDate");

            migrationBuilder.AddColumn<int>(
                name: "CurrentStreak",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HighestStreak",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "FoodId1",
                table: "PlannedMeals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MealTypeId1",
                table: "PlannedMeals",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "PasswordResetTokens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "PasswordResetTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "PasswordResetTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "IconName",
                table: "MealTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealPlans",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "MealPlans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "FiberPer100g",
                table: "Foods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ServingUnit",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "SodiumPer100g",
                table: "Foods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SugarPer100g",
                table: "Foods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FoodCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FoodCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "FoodCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "FoodId1",
                table: "FavoriteFoods",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFiber",
                table: "DailyIntakeSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "WaterMl",
                table: "DailyIntakeSummaries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PointsReward",
                table: "Badges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "MealTypes",
                columns: new[] { "Id", "DisplayOrder", "IconName", "Name" },
                values: new object[,]
                {
                    { 1, 1, "breakfast_icon", "Breakfast" },
                    { 2, 2, "lunch_icon", "Lunch" },
                    { 3, 3, "dinner_icon", "Dinner" },
                    { 4, 4, "snack_icon", "Snack" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlannedMeals_FoodId1",
                table: "PlannedMeals",
                column: "FoodId1");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedMeals_MealTypeId1",
                table: "PlannedMeals",
                column: "MealTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId1",
                table: "PasswordResetTokens",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlans_UserId1",
                table: "MealPlans",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteFoods_FoodId1",
                table: "FavoriteFoods",
                column: "FoodId1");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteFoods_UserId_FoodId",
                table: "FavoriteFoods",
                columns: new[] { "UserId", "FoodId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteFoods_Foods_FoodId1",
                table: "FavoriteFoods",
                column: "FoodId1",
                principalTable: "Foods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlans_Users_UserId1",
                table: "MealPlans",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetTokens_Users_UserId1",
                table: "PasswordResetTokens",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedMeals_Foods_FoodId",
                table: "PlannedMeals",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedMeals_Foods_FoodId1",
                table: "PlannedMeals",
                column: "FoodId1",
                principalTable: "Foods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedMeals_MealTypes_MealTypeId",
                table: "PlannedMeals",
                column: "MealTypeId",
                principalTable: "MealTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedMeals_MealTypes_MealTypeId1",
                table: "PlannedMeals",
                column: "MealTypeId1",
                principalTable: "MealTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteFoods_Foods_FoodId1",
                table: "FavoriteFoods");

            migrationBuilder.DropForeignKey(
                name: "FK_MealPlans_Users_UserId1",
                table: "MealPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetTokens_Users_UserId1",
                table: "PasswordResetTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedMeals_Foods_FoodId",
                table: "PlannedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedMeals_Foods_FoodId1",
                table: "PlannedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedMeals_MealTypes_MealTypeId",
                table: "PlannedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedMeals_MealTypes_MealTypeId1",
                table: "PlannedMeals");

            migrationBuilder.DropIndex(
                name: "IX_PlannedMeals_FoodId1",
                table: "PlannedMeals");

            migrationBuilder.DropIndex(
                name: "IX_PlannedMeals_MealTypeId1",
                table: "PlannedMeals");

            migrationBuilder.DropIndex(
                name: "IX_PasswordResetTokens_UserId1",
                table: "PasswordResetTokens");

            migrationBuilder.DropIndex(
                name: "IX_MealPlans_UserId1",
                table: "MealPlans");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteFoods_FoodId1",
                table: "FavoriteFoods");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteFoods_UserId_FoodId",
                table: "FavoriteFoods");

            migrationBuilder.DeleteData(
                table: "MealTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MealTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MealTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MealTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "CurrentStreak",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HighestStreak",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLogDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "FoodId1",
                table: "PlannedMeals");

            migrationBuilder.DropColumn(
                name: "MealTypeId1",
                table: "PlannedMeals");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "IconName",
                table: "MealTypes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "MealPlans");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "FiberPer100g",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "ServingUnit",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "SodiumPer100g",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "SugarPer100g",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "FoodCategories");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "FoodCategories");

            migrationBuilder.DropColumn(
                name: "FoodId1",
                table: "FavoriteFoods");

            migrationBuilder.DropColumn(
                name: "TotalFiber",
                table: "DailyIntakeSummaries");

            migrationBuilder.DropColumn(
                name: "WaterMl",
                table: "DailyIntakeSummaries");

            migrationBuilder.DropColumn(
                name: "PointsReward",
                table: "Badges");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "RefreshTokens",
                newName: "Revoked");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "RefreshTokens",
                newName: "Expires");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "PasswordResetTokens",
                newName: "Expires");

            migrationBuilder.RenameColumn(
                name: "SummaryDate",
                table: "DailyIntakeSummaries",
                newName: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "PasswordResetTokens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "PasswordResetTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FoodCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteFoods_UserId",
                table: "FavoriteFoods",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedMeals_Foods_FoodId",
                table: "PlannedMeals",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedMeals_MealTypes_MealTypeId",
                table: "PlannedMeals",
                column: "MealTypeId",
                principalTable: "MealTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
