namespace PRAPI.Services
{
    public interface ITokenService
    {
        bool CheckIfAdmin(string token);
        bool CheckIfAdminOrSameUser(string token, int id);
        string GetUserId(string token);
    }
}