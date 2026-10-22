using System.ComponentModel.DataAnnotations;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.Data;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.DTOModel
{
    public class SignUpDto
    {
        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        public string Account { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; } = string.Empty;

        public ERoleType Role { get; set; } = ERoleType.User;
    }
}
