namespace PRAPI.Services
{
    public interface ITokenService
    {
        bool CheckIfSameUser(string token, int id);
        string GetUserId(string token);
    }
}