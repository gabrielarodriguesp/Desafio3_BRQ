public interface IUserService
{
    Task<string> EnsureValidToken();
}
