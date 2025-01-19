using Microsoft.AspNetCore.DataProtection;

namespace OnlineShoppingPlatformApp.Business.DataProtection
{
    public class DataProtection : IDataProtection
    {
        private readonly IDataProtector _protector;

        public DataProtection(IDataProtectionProvider protector)
        {
            _protector = protector.CreateProtector("OnlineShoppingPlatformApp-security");
        }

        public string Protect(string text)
        {
            return _protector.Protect(text);
        }

        public string UnProtect(string protectedText)
        {
            return _protector.Unprotect(protectedText);
        }
    }
}