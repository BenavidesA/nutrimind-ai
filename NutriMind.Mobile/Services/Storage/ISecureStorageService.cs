namespace NutriMind.Mobile.Services.Storage;

public interface ISecureStorageService
{
    Task SaveTokenAsync(string token);

    Task<string?> GetTokenAsync();

    Task RemoveTokenAsync();

    Task SaveUserIdAsync(string userId);

    Task<string?> GetUserIdAsync();

    Task SaveUserNameAsync(string firstName, string lastName);

    Task<(string? FirstName, string? LastName)> GetUserNameAsync();
}