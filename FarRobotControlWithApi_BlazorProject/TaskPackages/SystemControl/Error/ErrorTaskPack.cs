using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Error.Interface;

namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Error
{
    public partial class ErrorTaskPack
    {
        readonly IErrorDataLibarary IDataLib;

        public ErrorTaskPack(IErrorDataLibarary IDataLib)
        {
            this.IDataLib = IDataLib;
        }

        const string info = "Inform";

        const string err = "Error";
    }

    public partial class ErrorTaskPack : IErrorTaskPack
    {
        public async Task NotifyDisconnect()
        {
            await IDataLib.NotifySysDisconect();
        }
    }
}
