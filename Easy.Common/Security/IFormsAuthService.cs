namespace Easy.Common.Security
{
    public interface IFormsAuthService
    {
        string GetFormsCookieName();

        void SignIn(IUser user, bool createPersistentCookie);

        void SignOut();
    }
}