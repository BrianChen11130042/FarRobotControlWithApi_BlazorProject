using System.ComponentModel.DataAnnotations;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.DTOModel
{
    public class SignInDto
    {
        [Required]
        public string Account { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
