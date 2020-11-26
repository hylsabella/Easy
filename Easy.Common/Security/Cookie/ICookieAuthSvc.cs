namespace Easy.Common.Security
{
    public interface ICookieAuthSvc
    {
        string GetFormsCookieName();

        void SignIn(IUser user, bool createPersistentCookie);

        void SignOut();
    }
}