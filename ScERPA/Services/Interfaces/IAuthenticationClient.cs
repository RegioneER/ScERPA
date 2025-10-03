namespace ScERPA.Services.Interfaces
{
    public interface IAuthenticationClient
    {
        public Task<ApiResult<string>> GetAuthTokenAsync(string? operationGuid, string? clientId, string? clientSecret);

    }
}
