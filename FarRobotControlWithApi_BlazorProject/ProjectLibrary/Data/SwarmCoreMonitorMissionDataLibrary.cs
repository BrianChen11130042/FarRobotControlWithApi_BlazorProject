using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data
{
    public partial class SwarmCoreMonitorMissionDataLibrary
    {
        readonly ILogTableOperate ILogTableOp;

        readonly IMissionTableOperate IMissionTableOp;

        readonly INLogObservable INLogObser;
        readonly IMissionObservable IMissionObser;

        public SwarmCoreMonitorMissionDataLibrary(ILogTableOperate ILogTableOp,
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

    public partial class SwarmCoreMonitorMissionDataLibrary
    {
        List<AmrMissionTable> _listRunningMission { get; set; } = new List<AmrMissionTable>();

        public List<AmrMissionTable> ListRunningMission
        {
            get
            {
                return _listRunningMission;
            }
            set
            {
                _listRunningMission = value;
            }
        }

        int _startedIndex { get; set; } = 0;

        public int StartedIndex
        {
            get
            {
                return _startedIndex;
            }
            set
            {
                _startedIndex = value;
            }
        }

        AmrMissionTable _targetRunningMission { get; set; } = new AmrMissionTable();

        public AmrMissionTable TargetRunningMission
        {
            get
            {
                return _targetRunningMission;
            }
            set
            {
                _targetRunningMission = value;
            }
        }
    }

    public partial class SwarmCoreMonitorMissionDataLibrary : ISwarmCoreMonitorMissionDataLibrary
    {
        public async Task<bool> GetRunningMissionTableList()
        {
            List<AmrMissionTable> listTable = await IMissionTableOp.GetRunningMissionTableList();

            if(listTable.Count != 0)
            {
                ListRunningMission.Clear();
                ListRunningMission = listTable;
                StartedIndex = 0;
            }
            else
            {
                ListRunningMission.Clear();
                StartedIndex = 0;
            }

            return true;
        }

        public async Task<bool> GetRunningMissionTableTarget()
        {
            TargetRunningMission = ListRunningMission[StartedIndex];

            StartedIndex = StartedIndex + 1;

            return true;
        }

        public async Task<bool> UpsertMissionTable()
        {
            bool _writeDb = TargetRunningMission.IsFinish || TargetRunningMission.IsCancel || TargetRunningMission.IsError;

            if (_writeDb)
            {
                var result = await IMissionTableOp.UpsertMissionTable(TargetRunningMission);

                if (!result.status)
                {
                    await WriteNLogError(result.msg);
                    return result.status;
                }
            }
            else
            {
                IMissionTableOp.UpsertMissionTableInQueue(TargetRunningMission);
            }

            foreach (FlowBase flow in TargetRunningMission.Flows)
            {
                switch (flow)
                {
                    case MoveFlowTable moveflow:
                        if (!await _upsertFlow<MoveFlowTable>(moveflow, _writeDb))
                        {
                            return false;
                        }
                        break;

                    case ChargeFlowTable chargeflow:
                        if (!await _upsertFlow<ChargeFlowTable>(chargeflow, _writeDb))
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        async Task<bool> _upsertFlow<T>(T flow, bool writeDb) where T : FlowBase
        {
            if(writeDb)
            {
                var result = await IMissionTableOp.UpsertFlow<T>(flow);

                if (!result.status)
                {
                    await WriteNLogError(result.msg);
                    return result.status;
                }

                return true;
            }
            else
            {
                IMissionTableOp.UpsertFlowInQueue<T>(flow);

                return true;
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
