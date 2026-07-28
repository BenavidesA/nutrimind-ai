using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Application.DTOs.Auth;
using NutriMind.Application.DTOs.Gamification;
using NutriMind.Application.DTOs.Profile;
using NutriMind.Application.Interfaces.Services;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces;
using NutriMind.Domain.Interfaces.Repositories;

namespace NutriMind.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;

    public UserController(IUnitOfWork unitOfWork, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized("Usuario no válido.");

        var result = await _authService.ChangePasswordAsync(userId, request);
        if (!result.IsSuccess)
            return BadRequest(result.ErrorMessage);

        return NoContent();
    }

    [HttpGet("progress")]
    public async Task<IActionResult> GetProgress()
    {
        // 1. Extract the user ID from the JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized("Usuario no válido.");

        // 2. Look up the user in the database
        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        // 3. Return only the streak and points
        return Ok(new
        {
            CurrentStreak = user.CurrentStreak,
            TotalPoints = user.TotalPoints
        });
    }

    [HttpGet("ranking")]
    public async Task<IActionResult> GetRanking([FromQuery] int top = 10)
    {
        var userRepo = _unitOfWork.Repository<User>();
        var users = await userRepo.GetAllAsync();

        var ranking = users
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.TotalPoints)
            .Take(top)
            .Select((u, index) => new
            {
                UserId = u.Id,
                Name = $"{u.FirstName} {u.LastName}".Trim(),
                TotalPoints = u.TotalPoints,
                Rank = index + 1
            });

        return Ok(ranking);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized("Usuario no válido.");

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        var profileRepo = _unitOfWork.Repository<UserProfile>();
        var profiles = await profileRepo.FindAsync(p => p.UserId == userId);
        var profile = profiles.FirstOrDefault();

        return Ok(new ProfileResponseDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Age = profile?.Age,
            Gender = profile?.Gender,
            HeightCm = profile?.HeightCm,
            WeightKg = profile?.WeightKg,
            ActivityLevel = profile?.ActivityLevel ?? Domain.Enums.ActivityLevel.Sedentary,
            DietaryGoal = profile?.DietaryGoal ?? Domain.Enums.DietaryGoal.MaintainWeight,
            University = profile?.University
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized("Usuario no válido.");

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        userRepo.Update(user);

        var profileRepo = _unitOfWork.Repository<UserProfile>();
        var profiles = await profileRepo.FindAsync(p => p.UserId == userId);
        var existingProfile = profiles.FirstOrDefault();
        bool isNew = existingProfile == null;

        var profile = existingProfile ?? new UserProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProfileImageUrl = string.Empty
        };

        profile.Age = dto.Age;
        profile.Gender = dto.Gender ?? string.Empty;
        profile.HeightCm = dto.HeightCm;
        profile.WeightKg = dto.WeightKg;
        profile.ActivityLevel = dto.ActivityLevel;
        profile.DietaryGoal = dto.DietaryGoal;
        profile.University = dto.University ?? string.Empty;

        if (isNew)
            await profileRepo.AddAsync(profile);
        else
            profileRepo.Update(profile);

        await _unitOfWork.SaveChangesAsync();

        return Ok(new ProfileResponseDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Age = profile.Age,
            Gender = profile.Gender,
            HeightCm = profile.HeightCm,
            WeightKg = profile.WeightKg,
            ActivityLevel = profile.ActivityLevel,
            DietaryGoal = profile.DietaryGoal,
            University = profile.University
        });
    }

    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized("Usuario no válido.");

        var userRepo = _unitOfWork.Repository<User>();
        var user = await userRepo.GetByIdAsync(userId);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        user.IsActive = false;
        userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("badges")]
    public async Task<IActionResult> GetBadges()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized("Usuario no válido.");

        var userBadgeRepo = _unitOfWork.Repository<UserBadge>();
        var userBadges = await userBadgeRepo.FindAsync(ub => ub.UserId == userId);

        var badgeIds = userBadges.Select(ub => ub.BadgeId).ToList();
        var badgeRepo = _unitOfWork.Repository<Badge>();
        var badges = await badgeRepo.FindAsync(b => badgeIds.Contains(b.Id));

        var result = userBadges
            .Join(badges, ub => ub.BadgeId, b => b.Id, (ub, b) => new BadgeResponseDto
            {
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                AwardedAt = ub.AwardedAt
            })
            .OrderByDescending(b => b.AwardedAt);

        return Ok(result);
    }
}