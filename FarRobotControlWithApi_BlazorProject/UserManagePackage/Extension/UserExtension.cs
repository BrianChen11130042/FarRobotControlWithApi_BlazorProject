using BitzArt.Blazor.Cookies;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.Application;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.EFModel;
using FarRobotControlWithApi_BlazorProject.UserManagePackage.Tool;
using Microsoft.EntityFrameworkCore;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.Extension
{
    public static class UserExtension
    {
        public static IHostApplicationBuilder AddUserFeaturePack(this IHostApplicationBuilder builder, string connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            builder.Services.AddDbContext<UserManageDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDataProtection();
            builder.AddBlazorCookies();

            builder.Services.AddSingleton<AuthTool>();
            builder.Services.AddScoped<AuthApplication>();
            builder.Services.AddSingleton<UserManageApplication>();

            return builder;
        }
    }
}
