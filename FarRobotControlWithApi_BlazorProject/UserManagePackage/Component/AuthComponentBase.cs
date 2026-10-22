using FarRobotControlWithApi_BlazorProject.UserManagePackage.Application;
using Microsoft.AspNetCore.Components;

namespace FarRobotControlWithApi_BlazorProject.UserManagePackage.Component
{
    public class AuthComponentBase : ComponentBase, IDisposable
    {

        [Inject]
        protected AuthApplication AuthApp { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            AuthApp.dgUserStatusChanged += handleUserChanged;

            await AuthApp.RetrieveUserInfoFromCookie();

            await base.OnInitializedAsync();
        }

        async Task handleUserChanged()
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            AuthApp.dgUserStatusChanged -= handleUserChanged;
        }
    }
}
