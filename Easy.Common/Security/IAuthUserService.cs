namespace Easy.Common.Security
{
    public interface IAuthUserService
    {
        UserModel GetByToken(string token);
    }
}