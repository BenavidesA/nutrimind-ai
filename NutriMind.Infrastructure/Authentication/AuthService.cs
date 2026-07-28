using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Auth;
using NutriMind.Application.Interfaces.Services;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces;
using NutriMind.Domain.Interfaces.Repositories;

namespace NutriMind.Infrastructure.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var users = await userRepo.FindAsync(u => u.Email == request.Email, cancellationToken);
            var existingUser = users.FirstOrDefault();

            if (existingUser != null)
                return Result<AuthResponseDto>.Failure("El correo electr�nico ya est� registrado.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "Student",
                IsActive = true
            };

            await userRepo.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await LoginAsync(new LoginDto { Email = request.Email, Password = request.Password }, cancellationToken);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto request, CancellationToken cancellationToken = default)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var users = await userRepo.FindAsync(u => u.Email == request.Email, cancellationToken);
            var user = users.FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Result<AuthResponseDto>.Failure("Credenciales incorrectas.");

            if (!user.IsActive)
                return Result<AuthResponseDto>.Failure("Esta cuenta ha sido eliminada.");

            var token = GenerateJwtToken(user);
            var refreshToken = await IssueRefreshTokenAsync(user.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new AuthResponseDto
            {
                UserId = user.Id.ToString(), // We convert the user's Guid to a string so it matches the DTO
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = token,
                RefreshToken = refreshToken.Token
            };

            return Result<AuthResponseDto>.Success(response);
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result<AuthResponseDto>.Failure("El refresh token es requerido.");

            var tokenRepo = _unitOfWork.Repository<RefreshToken>();
            var tokens = await tokenRepo.FindAsync(t => t.Token == request.RefreshToken, cancellationToken);
            var storedToken = tokens.FirstOrDefault();

            if (storedToken == null || !storedToken.IsActive)
                return Result<AuthResponseDto>.Failure("El refresh token no es válido o ha expirado.");

            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetByIdAsync(storedToken.UserId, cancellationToken);

            if (user == null || !user.IsActive)
                return Result<AuthResponseDto>.Failure("El usuario asociado no es válido.");

            // Rotation: the used token is revoked and a new, single-use one is issued.
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            tokenRepo.Update(storedToken);

            var newRefreshToken = await IssueRefreshTokenAsync(user.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var accessToken = GenerateJwtToken(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        public async Task<Result<bool>> LogoutAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                return Result<bool>.Failure("El identificador de usuario no es válido.");

            await RevokeActiveRefreshTokensAsync(userGuid, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto request, CancellationToken cancellationToken = default)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var users = await userRepo.FindAsync(u => u.Email == request.Email, cancellationToken);
            var user = users.FirstOrDefault();

            // Generic response, always successful, whether or not the account exists — prevents
            // user enumeration (the response doesn't reveal whether an email is registered).
            if (user == null || !user.IsActive)
                return Result<bool>.Success(true);

            var code = Random.Shared.Next(100000, 999999).ToString();
            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                Token = code,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PasswordResetToken>().AddAsync(resetToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // The code has already been saved before attempting to send it — an email failure
            // (e.g. Resend sandbox restriction) shouldn't break the flow.
            await _emailService.SendPasswordResetCodeAsync(user.Email, code, cancellationToken);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> VerifyResetCodeAsync(VerifyResetCodeDto request, CancellationToken cancellationToken = default)
        {
            var token = await FindValidResetTokenAsync(request.Email, request.Code, cancellationToken);
            return token != null
                ? Result<bool>.Success(true)
                : Result<bool>.Failure("El código es inválido o ha expirado.");
        }

        public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request, CancellationToken cancellationToken = default)
        {
            var token = await FindValidResetTokenAsync(request.Email, request.Code, cancellationToken);
            if (token == null)
                return Result<bool>.Failure("El código es inválido o ha expirado.");

            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetByIdAsync(token.UserId, cancellationToken);
            if (user == null)
                return Result<bool>.Failure("Usuario no encontrado.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            userRepo.Update(user);

            token.IsUsed = true;
            _unitOfWork.Repository<PasswordResetToken>().Update(token);

            // Same as with the authenticated password change: if someone else had a session
            // open with the old password, it gets closed.
            await RevokeActiveRefreshTokensAsync(user.Id, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto request, CancellationToken cancellationToken = default)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result<bool>.Failure("Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return Result<bool>.Failure("La contraseña actual no es correcta.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            userRepo.Update(user);

            await RevokeActiveRefreshTokensAsync(userId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        private async Task<PasswordResetToken?> FindValidResetTokenAsync(string email, string code, CancellationToken cancellationToken)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var users = await userRepo.FindAsync(u => u.Email == email, cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
                return null;

            var tokenRepo = _unitOfWork.Repository<PasswordResetToken>();
            var tokens = await tokenRepo.FindAsync(
                t => t.UserId == user.Id && t.Token == code && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

            return tokens.FirstOrDefault();
        }

        private async Task RevokeActiveRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            var tokenRepo = _unitOfWork.Repository<RefreshToken>();
            var activeTokens = await tokenRepo.FindAsync(t => t.UserId == userId && !t.IsRevoked, cancellationToken);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                tokenRepo.Update(token);
            }
        }

        private const int RefreshTokenLifetimeDays = 30;

        private async Task<RefreshToken> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenLifetimeDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken, cancellationToken);
            return refreshToken;
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "SuperSecretKeyDefaultNutriMindAI2026!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}