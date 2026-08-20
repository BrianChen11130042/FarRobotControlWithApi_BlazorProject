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
        List<AmrMissionTable> _listStartedMission { get; set; } = new List<AmrMissionTable>();

        public List<AmrMissionTable> ListStartedMission
        {
            get
            {
                return _listStartedMission;
            }
            set
            {
                _listStartedMission = value;
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

        AmrMissionTable _targetStartedMission { get; set; } = new AmrMissionTable();

        public AmrMissionTable TargetStartedMission
        {
            get
            {
                return _targetStartedMission;
            }
            set
            {
                _targetStartedMission = value;
            }
        }
    }

    public partial class SwarmCoreMonitorMissionDataLibrary : ISwarmCoreMonitorMissionDataLibrary
    {
        public async Task<bool> GetStartedMissionTableList()
        {
            List<AmrMissionTable> listTable = await IMissionTableOp.GetStartedMissionTableList();

            if(listTable.Count != 0)
            {
                ListStartedMission.Clear();
                ListStartedMission = listTable;
                StartedIndex = 0;
            }
            else
            {
                ListStartedMission.Clear();
                StartedIndex = 0;
            }

            return true;
        }

        public async Task<bool> GetStartedMissionTableTarget()
        {
            TargetStartedMission = ListStartedMission[StartedIndex];

            StartedIndex = StartedIndex + 1;

            return true;
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
