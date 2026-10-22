using FarRobotControlWithApi_BlazorProject.UserManagePackage.Data;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.DTOModel
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Account { get; set; } = string.Empty;

        public ERoleType Role { get; set; } = ERoleType.Guest;
    }
}
