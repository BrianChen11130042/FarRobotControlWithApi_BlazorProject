using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data
{
    public partial class ErrorDataLibrary
    {
        readonly ILogTableOperate ILogTableOp;

        readonly ISystemControlObservable ISysControlObser;
        readonly INLogObservable INLogObser;

        public ErrorDataLibrary(ILogTableOperate ILogTableOp,
                                ISystemControlObservable ISysControlObser,
                                INLogObservable INLogObser)
        {
            this.ILogTableOp = ILogTableOp;
            this.ISysControlObser = ISysControlObser;
            this.INLogObser = INLogObser;
        }

        async Task _writeNLogError(string log)
        {
            await INLogObser.NotifyNLog(EStatus.Error, log);
        }

        async Task _writeNLogInform(string log)
        {
            await INLogObser.NotifyNLog(EStatus.Info, log);
        }
    }

    public partial class ErrorDataLibrary : IErrorDataLibarary
    {
        public async Task NotifySysDisconect()
        {
            await ISysControlObser.NotifyDisconnect();
        }

        public async Task WriteNLogError(string log)
        {
            await _writeNLogError(log);
        }

        public async Task WriteNLogInform(string log)
        {
            await _writeNLogInform(log);
        }
    }
}
