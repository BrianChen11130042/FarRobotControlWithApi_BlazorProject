using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data
{
    public partial class SwarmCoreSetMissionDataLibrary
    {
        readonly ILogTableOperate ILogTableOp;

        readonly IMissionTableOperate IMissionTableOp;

        readonly INLogWritterObservable INLogWritter;


        public SwarmCoreSetMissionDataLibrary(ILogTableOperate ILogTableOp,
                                              IMissionTableOperate IMissionTableOp,
                                              INLogWritterObservable INLogWritter)
        {
            this.ILogTableOp = ILogTableOp;
            this.IMissionTableOp = IMissionTableOp;
            this.INLogWritter = INLogWritter;
        }

        async Task _writeNLogError(string log)
        {
            await INLogWritter.NotifyNLog(EStatus.Error, log);
        }

        async Task _writeNLogInform(string log)
        {
            await INLogWritter.NotifyNLog(EStatus.Info, log);
        }

    }

    public partial class SwarmCoreSetMissionDataLibrary : ISwarmCoreSetMissionDataLibrary
    {


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
