using System;

namespace NutriMind.Application.DTOs.Gamification;

public class BadgeResponseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
}
