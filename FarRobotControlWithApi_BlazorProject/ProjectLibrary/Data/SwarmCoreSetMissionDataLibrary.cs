using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data
{
    public partial class SwarmCoreSetMissionDataLibrary
    {
        readonly ILogTableOperate ILogTableOp;

        readonly IMissionTableOperate IMissionTableOp;

        readonly INLogObservable INLogObser;
        readonly IMissionObservable IMissionObser;

        public SwarmCoreSetMissionDataLibrary(ILogTableOperate ILogTableOp,
                                              IMissionTableOperate IMissionTableOp,
                                              INLogObservable INLogObser,
                                              IMissionObservable IMissionObser)
        {
            this.ILogTableOp = ILogTableOp;
            this.IMissionTableOp = IMissionTableOp;
            this.INLogObser = INLogObser;
            this.IMissionObser = IMissionObser;
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

    public partial class SwarmCoreSetMissionDataLibrary
    {

        AmrMissionTable _armMission { get; set; } = new AmrMissionTable();

        public AmrMissionTable AmrMission
        {
            get
            {
                return _armMission;
            }
            set
            {
                _armMission = value;
            }
        }

    }

    public partial class SwarmCoreSetMissionDataLibrary : ISwarmCoreSetMissionDataLibrary
    {
        public async Task<bool> GetNextMissionTable()
        {
            AmrMissionTable table = await IMissionTableOp.GetNextMissionTable();

            if(table != null)
            {
                AmrMission = table;
            }
            else
            {
                AmrMission = new AmrMissionTable();
            }

            return true;
        }

        public async Task<bool> UpsertMissionTable()
        {
            var result = await IMissionTableOp.UpsertMissionTable(AmrMission);

            if (!result.status)
            {
                await WriteNLogError(result.msg);
                return result.status;
            }

            foreach (FlowBase flow in AmrMission.Flows)
            {
                switch(flow)
                {
                    case MoveFlowTable moveflow:
                        if(!await _upsertFlow<MoveFlowTable>(moveflow))
                        {
                            return false;
                        }
                        break;

                    case ChargeFlowTable chargeflow:
                        if(! await _upsertFlow<ChargeFlowTable>(chargeflow))
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        async Task<bool> _upsertFlow<T>(T flow) where T : FlowBase
        {
            var result = await IMissionTableOp.UpsertFlow<T>(flow);

            if (result.status)
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
