namespace NutriMind.Mobile.Services.Storage;

public class SecureStorageService : ISecureStorageService
{
    private const string TokenKey = "jwt_token";
    private const string UserIdKey = "user_id";
    private const string FirstNameKey = "user_first_name";
    private const string LastNameKey = "user_last_name";

    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.Default.GetAsync(TokenKey);
    }

    public Task RemoveTokenAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public async Task SaveUserIdAsync(string userId)
    {
        await SecureStorage.Default.SetAsync(UserIdKey, userId);
    }

    public async Task<string?> GetUserIdAsync()
    {
        return await SecureStorage.Default.GetAsync(UserIdKey);
    }

    public async Task SaveUserNameAsync(string firstName, string lastName)
    {
        await SecureStorage.Default.SetAsync(FirstNameKey, firstName);
        await SecureStorage.Default.SetAsync(LastNameKey, lastName);
    }

    public async Task<(string? FirstName, string? LastName)> GetUserNameAsync()
    {
        var firstName = await SecureStorage.Default.GetAsync(FirstNameKey);
        var lastName = await SecureStorage.Default.GetAsync(LastNameKey);
        return (firstName, lastName);
    }
}