namespace PRAPI.Services
{
    public interface ITokenService
    {
        bool CheckIfAdmin(string token);
        bool CheckIfSameUser(string token, int id);
        string GetUserId(string token);
    }
}