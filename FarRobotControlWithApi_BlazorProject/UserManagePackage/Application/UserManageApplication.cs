using FarRobotControlWithApi_BlazorProject.UserManagePackage.DTOModel;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.EFModel;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.Tool;
using Microsoft.EntityFrameworkCore;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.Application
{
    public partial class UserManageApplication
    {
        readonly AuthTool authTool;
        readonly IServiceScopeFactory IScopeFactory;

        public UserManageApplication(AuthTool authTool,
                                     IServiceScopeFactory IScopeFactory)
        {
            this.authTool = authTool;
            this.IScopeFactory = IScopeFactory;
        }

        public dgUserInfoChanged dgUserInfoChanged;
    }

    public delegate Task dgUserInfoChanged();

    public partial class UserManageApplication
    {
        public async Task<(bool status, string msg, List<UserInfoDto> listUserInfoDto)> GetUserInfo()
        {
            var result = await _getListUserInfoTable();

            if(!result.status)
            {
                return (result.status, result.msg, new List<UserInfoDto>());
            }

            List<UserInfoDto> list = new List<UserInfoDto>();

            foreach(var table in result.listUserInfoTable)
            {
                list.Add(new UserInfoDto()
                {
                    Id = table.Id,
                    UserName = table.UserName,
                    Account = table.Account,
                    Role = table.Role,
                });
            }

            return (true, string.Empty, list);
        }

        public async Task<(bool status, string msg)> AddUserInfo(SignUpDto signUp)
        {
            UserInfoTable data = new UserInfoTable()
            {
                Id = Guid.NewGuid(),
                UserName = signUp.UserName,
                Account = signUp.Account,
                PasswordHash = authTool.HashPassword(signUp.Password),
                Role = signUp.Role,
            };

            var result = await _insertUserInfoTable(data);

            if(result.status)
            {
                dgUserInfoChanged?.Invoke();
                return (result.status, $"Add user {signUp.UserName} success");
            }
            else
            {
                return (result.status, result.msg);
            }
        }

        public async Task<(bool status, string msg)> DeleteUserInfo(Guid id)
        {
            var result = await _deleteUserInfoTable(id);

            if(result.status)
            {
                dgUserInfoChanged?.Invoke();
                return (result.status, $"Delete user success");
            }
            else
            {
                return (result.status, result.msg);
            }
        }
    }

    public partial class UserManageApplication
    {
        async Task<(bool status, string msg, List<UserInfoTable> listUserInfoTable)> _getListUserInfoTable()
        {
            try
            {
                using (var scope = IScopeFactory.CreateScope())
                {
                    UserManageDbContext context = scope.ServiceProvider.GetRequiredService<UserManageDbContext>();

                    List<UserInfoTable> list = await context.UserInfoTables.AsNoTracking().ToListAsync();

                    return (true, string.Empty, list);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, new List<UserInfoTable>());
            }
        }

        async Task<(bool status, string msg)> _insertUserInfoTable(UserInfoTable data)
        {
            try
            {
                using (var scope = IScopeFactory.CreateScope())
                {
                    UserManageDbContext context = scope.ServiceProvider.GetRequiredService<UserManageDbContext>();

                    if(await context.UserInfoTables.AnyAsync(x => x.Account == data.Account))
                    {
                        return (false, $"Account {data.Account} already exist");
                    }

                    context.UserInfoTables.Add(data);

                    await context.SaveChangesAsync();

                    return (true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        async Task<(bool status, string msg)> _deleteUserInfoTable(Guid id)
        {
            try
            {
                using (var scope = IScopeFactory.CreateScope())
                {
                    UserManageDbContext context = scope.ServiceProvider.GetRequiredService<UserManageDbContext>();

                    var target = await context.UserInfoTables.FirstOrDefaultAsync(x => x.Id == id);

                    if (target is null)
                    {
                        return (false, "User not found");
                    }

                    context.UserInfoTables.Remove(target);

                    await context.SaveChangesAsync();

                    return (true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
