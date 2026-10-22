using System.ComponentModel.DataAnnotations;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.Data;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.EFModel
{
    public class UserInfoTable
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(500)]
        public string UserName { get; set; }

        [MaxLength(500)]
        public string Account { get; set; }

        public string PasswordHash { get; set; }

        public ERoleType Role { get; set; }
    }
}
