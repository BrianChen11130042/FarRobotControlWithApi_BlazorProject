using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data
{
    public partial class InitialDataLibrary
    {
        readonly ILogTableOperate ILogTableOp;
        readonly IMissionTableOperate IMissionTableOp;

        readonly ISystemControlObservable ISysControlObser;
        readonly IMissionObservable IMissionObser;
        readonly INLogObservable INLogObser;

        public InitialDataLibrary(ILogTableOperate ILogTableOp,
                                  IMissionTableOperate IMissionTableOp,
                                  ISystemControlObservable ISysControlObser,
                                  IMissionObservable IMissionObser,
                                  INLogObservable INLogObser)
        {
            this.ILogTableOp = ILogTableOp;
            this.IMissionTableOp = IMissionTableOp;
            this.ISysControlObser = ISysControlObser;
            this.IMissionObser = IMissionObser;
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

    public partial class InitialDataLibrary : IInitialDataLibrary
    {
        public async Task<bool> InitAmrMissionInQueue()
        {
            var result = await IMissionTableOp.InitAmrMissionInQueue();

            if(result.status)
            {
                return result.status;
            }
            else
            {
                await WriteNLogError(result.msg);
                return result.status;
            }
        }

        public async Task NotifyMissionUpdated()
        {
            await IMissionObser.NotifyMissionUpdated(IMissionTableOp.listAmrMissionInQueue);
        }

        public async Task NotifyIntialResult(bool success, string msg)
        {
            await ISysControlObser.NotifyInitialResult(success, msg);
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
