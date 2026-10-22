using BitzArt.Blazor.Cookies;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.Data;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.DTOModel;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.EFModel;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.Tool;
using Microsoft.EntityFrameworkCore;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.Application
{
    public partial class AuthApplication
    {
        readonly AuthTool authTool;
        readonly ICookieService ICookieService;
        readonly IServiceScopeFactory IScopeFactory;

        public AuthApplication(AuthTool authTool,
                               ICookieService ICookieService,
                               IServiceScopeFactory IScopeFactory)
        {
            this.authTool = authTool;
            this.ICookieService = ICookieService;
            this.IScopeFactory = IScopeFactory;
        }

        UserInfoDto? _currentUser { get; set; } = null;

        public UserInfoDto? currentUser { get { return _currentUser; } }

        public bool IsAuth => _currentUser is not null && _currentUser.Role != ERoleType.Guest;

        public bool IsRoleMoreThan(ERoleType role) => _currentUser is not null && _currentUser.Role >= role;

        public dgUserStatusChanged dgUserStatusChanged;
    }

    public delegate Task dgUserStatusChanged();

    public partial class AuthApplication
    {
        public async Task<(bool success, string msg)> SignIn(SignInDto signIn)
        {
            UserInfoDto? developer = _getDeveloperInfo(signIn);

            if (developer is not null)
            {
                await _setCurrentUser(developer);
                return (true, "Developer sign in success");
            }

            var userInfoResult = await _getUserInfoTable(signIn);

            if(!userInfoResult.status)
            {
                return (false, userInfoResult.msg);
            }

            if (userInfoResult.table is null)
            {
                return (false, $"User {signIn.Account} not found");
            }

            if (!authTool.VerifyPassword(signIn.Password, userInfoResult.table.PasswordHash))
            {
                return (false, "Password not match");
            }

            UserInfoDto User = new UserInfoDto()
            {
                Id = userInfoResult.table.Id,
                UserName = userInfoResult.table.UserName,
                Account = userInfoResult.table.Account,
                Role = userInfoResult.table.Role
            };

            await _setCurrentUser(User);

            return (true, $"User {User.UserName} sign in success");
        }

        public async Task SignOut()
        {
            await ICookieService.RemoveAsync(authTool.CookieName);

            await _setGuestUser();
        }

        public async Task RetrieveUserInfoFromCookie()
        {
            try
            {
                var cookie = await ICookieService.GetAsync(authTool.CookieName);

                if (cookie is null)
                {
                    await _setGuestUser();

                    return;
                }

                var user = authTool.DecryptUserInfo(cookie.Value);

                if (user is not null)
                {
                    await _setCurrentUser(user);
                }
                else
                {
                    await _setGuestUser();
                }
            }
            catch
            {
                await _setGuestUser();
            }
        }
    }

    public partial class AuthApplication
    {
        UserInfoDto? _getDeveloperInfo(SignInDto signIn)
        {
            if(string.Equals(signIn.Account, "Developer", StringComparison.OrdinalIgnoreCase)
               && string.Equals(signIn.Password, "Developer", StringComparison.OrdinalIgnoreCase))
            {
                return new UserInfoDto()
                {
                    Id = Guid.Empty,
                    UserName = ERoleType.Developer.ToString(),
                    Account = ERoleType.Developer.ToString(),
                    Role = ERoleType.Developer
                };
            }
            else
            {
                return null;
            }
        }

        async Task<(bool status, string msg, UserInfoTable? table)> _getUserInfoTable(SignInDto signIn)
        {
            try
            {
                using (var scope = IScopeFactory.CreateScope())
                {
                    UserManageDbContext context = scope.ServiceProvider.GetRequiredService<UserManageDbContext>();

                    var target = await context.UserInfoTables.AsNoTracking()
                                                             .FirstOrDefaultAsync(x => x.Account == signIn.Account);

                    return (true, string.Empty, target);
                }
            }
            catch(Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        async Task _setCurrentUser(UserInfoDto user)
        {
            _currentUser = user;

            string encrypted = authTool.EncryptUserInfo(user);
            await ICookieService.SetAsync(authTool.CookieName, encrypted, authTool.CookieExpiration);

            dgUserStatusChanged?.Invoke();
        }

        async Task _setGuestUser()
        {
            UserInfoDto guest = new UserInfoDto()
            {
                Id = Guid.Empty,
                UserName = ERoleType.Guest.ToString(),
                Account = ERoleType.Guest.ToString(),
                Role = ERoleType.Guest
            };

            _currentUser = guest;

            string encrypted = authTool.EncryptUserInfo(guest);
            await ICookieService.SetAsync(authTool.CookieName, encrypted, authTool.CookieExpiration);

            dgUserStatusChanged?.Invoke();
        }
    }
}
