using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.DTOModel;
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

    public partial class InitialDataLibrary
    {
        Dictionary<string, List<ArtifactInformDto>> _dcAmrArtifactMap { get; set; } = new Dictionary<string, List<ArtifactInformDto>>();

        public Dictionary<string, List<ArtifactInformDto>> DcAmrArtifactMap
        {
            get
            {
                return _dcAmrArtifactMap;
            }
            set
            {
                _dcAmrArtifactMap = value;
            }
        }


        List<string> _listFlowName { get; set; } = new List<string>();

        public List<string> ListFlowName
        {
            get
            {
                return _listFlowName;
            }
            set
            {
                _listFlowName = value;
            }
        }

        List<string> _listCellName { get; set; } = new List<string>();

        public List<string> ListCellName
        {
            get
            {
                return _listCellName;
            }
            set
            {
                _listCellName = value;
            }
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

        public async Task NotifyMissionParamUpdated()
        {
            await IMissionObser.NotifyMissionParamUpdated(ListFlowName, ListCellName, DcAmrArtifactMap);
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
