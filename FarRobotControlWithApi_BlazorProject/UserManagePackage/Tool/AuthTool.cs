using System.Text.Json;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.DTOModel;
using Microsoft.AspNetCore.DataProtection;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.Tool
{
    public class AuthTool
    {
        readonly IDataProtector IProtector;
        readonly string cookieName = "FarRobotControlAuthCookie";
        readonly string encryptKey = "FarRobotControlAuthEncryptKey";
        readonly int expiredHour = 1;

        public AuthTool(IDataProtectionProvider provider)
        {
            IProtector = provider.CreateProtector(encryptKey);
        }

        public string CookieName => cookieName;
        public DateTimeOffset CookieExpiration => DateTimeOffset.Now.AddHours(expiredHour);

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        public string EncryptUserInfo(UserInfoDto userInfo)
        {
            string json = JsonSerializer.Serialize(userInfo);
            return IProtector.Protect(json);
        }

        public UserInfoDto? DecryptUserInfo(string encrypted)
        {
            string json = IProtector.Unprotect(encrypted);
            return JsonSerializer.Deserialize<UserInfoDto>(json);
        }
    }
}
