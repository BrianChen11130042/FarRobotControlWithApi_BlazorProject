using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.DTOModel;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;

namespace FarRobotControlWithApi_BlazorProject.Application
{
    public partial class FarRobotMissionApplication
    {
        readonly IMissionTableOperate IMissionTableOp;
        readonly IInitialDataLibrary IInitDataLib;
        readonly INLogObservable IObserverLib;

        public FarRobotMissionApplication(IMissionTableOperate IMissionTableOp,
                                          IInitialDataLibrary IInitDataLib,
                                          INLogObservable IObserverLib)
        {
            this.IMissionTableOp = IMissionTableOp;
            this.IInitDataLib = IInitDataLib;
            this.IObserverLib = IObserverLib;
        }
    }

    public partial class FarRobotMissionApplication
    {
        public async Task<List<AmrMissionTable>> GetAmrMissionInQueue()
        {
            return IMissionTableOp.listAmrMissionInQueue;
        }

        public async Task<(List<string> flowNames, List<string> cellNames,
                           Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts, 
                           List<ArtifactInformDto> extArtifacts)> GetAmrMissionParam()
        {
            List<string> flows = IInitDataLib.ListFlowName;
            List<string> cells = IInitDataLib.ListCellName;
            Dictionary<string, List<ArtifactInformDto>> amrEmbs = IInitDataLib.DcAmrWithEmbArtifact;
            List<ArtifactInformDto> exts = IInitDataLib.ListExtArtifact;

            return (flows, cells, amrEmbs, exts);
        }

        public async Task<bool> SetMission(AmrMissionTable mission)
        {
            var result = await IMissionTableOp.UpsertMissionTable(mission);

            if (result.status)
            {
                return result.status;
            }
            else
            {
                await IObserverLib.NotifyNLog(EStatus.Error, result.msg);
                return result.status;
            }
        }

        public async Task<bool> CancelMission(Guid missionId)
        {

            AmrMissionTable? mission = IMissionTableOp.listAmrMissionInQueue.FirstOrDefault(x => x.Id == missionId
                                                                                              && x.IsFinish == false
                                                                                              && x.IsCancel == false
                                                                                              && (string.Equals(x.MissionState,
                                                                                                                EMissionState.FAILED.ToString(),
                                                                                                                StringComparison.OrdinalIgnoreCase) ||
                                                                                                  string.Equals(x.MissionState,
                                                                                                                EMissionState.RUNNING.ToString(),
                                                                                                                StringComparison.OrdinalIgnoreCase) ||
                                                                                                  string.Equals(x.MissionState,
                                                                                                                EMissionState.DISPATCH_REQUEST.ToString(),
                                                                                                                StringComparison.OrdinalIgnoreCase)));

            if (mission == null)
            {
                await IObserverLib.NotifyNLog(EStatus.Error, "Mission not found in queue");
                return false;
            }

            mission.MissionState = EMissionState.CANCEL_REQUEST.ToString();

            if (!await SetMission(mission))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RetryMission(Guid missionId)
        {
            AmrMissionTable? mission = IMissionTableOp.listAmrMissionInQueue.FirstOrDefault(x => x.Id == missionId
                                                                                              && x.IsFinish == false
                                                                                              && x.IsCancel == false
                                                                                              && x.Flows.Any(f => f.IsError && !f.IsFinish && !f.IsCancel)
                                                                                              && string.Equals(x.MissionState,
                                                                                                               EMissionState.FAILED.ToString(),
                                                                                                               StringComparison.OrdinalIgnoreCase));

            if (mission == null)
            {
                await IObserverLib.NotifyNLog(EStatus.Error, "Mission not found in queue");
                return false;
            }

            List<FlowBase> listFailFlow = mission.Flows.Where(f => f.IsError && !f.IsFinish && !f.IsCancel)
                                                       .OrderBy(f => f.EstablishTime)
                                                       .ToList();

            List<FlowBase> listRetryFlow = _getListRetryFlow(listFailFlow);

            if (!await _setListRetryFlow(listRetryFlow))
            {
                return false;
            }

            mission.FlowCount = mission.FlowCount + listRetryFlow.Count;
            mission.MissionState = EMissionState.RETRY_REQUEST.ToString();

            if (!await SetMission(mission))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ContinueMission(Guid missionId)
        {
            AmrMissionTable? mission = IMissionTableOp.listAmrMissionInQueue.FirstOrDefault(x => x.Id == missionId
                                                                                              && x.IsFinish == false
                                                                                              && x.IsCancel == false
                                                                                              && string.Equals(x.MissionState,
                                                                                                               EMissionState.FAILED.ToString(),
                                                                                                               StringComparison.OrdinalIgnoreCase)
                                                                                              && x.Flows.Any(f => f.IsStart && f.IsError && !f.IsFinish && !f.IsCancel));

            if (mission == null)
            {
                await IObserverLib.NotifyNLog(EStatus.Error, "Mission not found in queue");
                return false;
            }

            mission.MissionState = EMissionState.CONTINUE_REQUEST.ToString();

            if (!await SetMission(mission))
            {
                return false;
            }

            return true;
        }
    }

    public partial class FarRobotMissionApplication
    {
        List<FlowBase> _getListRetryFlow(List<FlowBase> listFailFlow)
        {
            DateTime now = DateTime.Now;
            List<FlowBase> listRetryFlow = new List<FlowBase>();

            foreach (FlowBase failFlow in listFailFlow)
            {
                now = now.AddMilliseconds(1);

                switch (failFlow)
                {
                    case MoveFlowTable move:

                        listRetryFlow.Add(new MoveFlowTable()
                        {
                            Id = Guid.NewGuid(),
                            MissionId = move.MissionId,
                            AmrSerialNumber = move.AmrSerialNumber,
                            Priority = 5,
                            EstablishTime = now,
                            CellName = move.CellName
                        });

                        break;

                    case ChargeFlowTable charge:

                        listRetryFlow.Add(new ChargeFlowTable()
                        {
                            Id = Guid.NewGuid(),
                            MissionId = charge.MissionId,
                            AmrSerialNumber = charge.AmrSerialNumber,
                            Priority = 5,
                            EstablishTime = now,
                            CellName = charge.CellName,
                            Percentage = charge.Percentage
                        });

                        break;

                    case MoveArtifactFlowTable moveArtifact:

                        listRetryFlow.Add(new MoveArtifactFlowTable()
                        {
                            Id = Guid.NewGuid(),
                            MissionId = moveArtifact.MissionId,
                            AmrSerialNumber = moveArtifact.AmrSerialNumber,
                            Priority = 5,
                            EstablishTime = now,
                            CellName = moveArtifact.CellName,
                            EmbArtifactId = moveArtifact.EmbArtifactId,
                            StartParam = moveArtifact.StartParam,
                            FinishParam = moveArtifact.FinishParam,
                            ErrorParam = moveArtifact.ErrorParam
                        });

                        break;

                    case MoveArtifactsFlowTable moveArtifacts:

                        listRetryFlow.Add(new MoveArtifactsFlowTable()
                        {
                            Id = Guid.NewGuid(),
                            MissionId = moveArtifacts.MissionId,
                            AmrSerialNumber = moveArtifacts.AmrSerialNumber,
                            Priority = 5,
                            EstablishTime = now,
                            CellName = moveArtifacts.CellName,
                            EmbArtifactId = moveArtifacts.EmbArtifactId,
                            EmbStartParam = moveArtifacts.EmbStartParam,
                            EmbFinishParam = moveArtifacts.EmbFinishParam,
                            EmbErrorParam = moveArtifacts.EmbErrorParam,
                            ExtArtifactId = moveArtifacts.ExtArtifactId,
                            ExtStartParam = moveArtifacts.ExtStartParam,
                            ExtFinishParam = moveArtifacts.ExtFinishParam,
                            ExtErrorParam = moveArtifacts.ExtErrorParam,
                        });

                        break;

                    case RobotWinderFlowTable robotWinder:

                        listRetryFlow.Add(new RobotWinderFlowTable()
                        {
                            Id = Guid.NewGuid(),
                            MissionId = robotWinder.MissionId,
                            AmrSerialNumber = robotWinder.AmrSerialNumber,
                            Priority = 5,
                            EstablishTime = now,
                            CellName = robotWinder.CellName,
                            WinderUnlockArtifactId = robotWinder.WinderUnlockArtifactId,
                            WinderUnlockFinishParam = robotWinder.WinderUnlockFinishParam,
                            WinderUnlockErrorParam = robotWinder.WinderUnlockErrorParam,
                            TmRobotArtifactId = robotWinder.TmRobotArtifactId,
                            TmRobotStartParam = robotWinder.TmRobotStartParam,
                            TmRobotFinishParam = robotWinder.TmRobotFinishParam,
                            TmRobotErrorParam = robotWinder.TmRobotErrorParam,
                            WinderLockArtifactId = robotWinder.WinderLockArtifactId,
                            WinderLockFinishParam = robotWinder.WinderLockFinishParam,
                            WinderLockErrorParam = robotWinder.WinderLockErrorParam
                        });

                        break;

                    default:
                        break;
                }
            }

            return listRetryFlow;
        }

        async Task<bool> _setListRetryFlow(IEnumerable<FlowBase> listRetryFlow)
        {
            foreach (FlowBase flow in listRetryFlow)
            {
                switch (flow)
                {
                    case MoveFlowTable move:

                        var moveResult = await IMissionTableOp.UpsertFlow(move);

                        if (!moveResult.status)
                        {
                            await IObserverLib.NotifyNLog(EStatus.Error, moveResult.msg);
                            return moveResult.status;
                        }
                        break;
                    case ChargeFlowTable charge:

                        var chargeResult = await IMissionTableOp.UpsertFlow(charge);

                        if (!chargeResult.status)
                        {
                            await IObserverLib.NotifyNLog(EStatus.Error, chargeResult.msg);
                            return chargeResult.status;
                        }
                        break;

                    case MoveArtifactFlowTable moveArtifact:

                        var moveArtifactResult = await IMissionTableOp.UpsertFlow(moveArtifact);

                        if (!moveArtifactResult.status)
                        {
                            await IObserverLib.NotifyNLog(EStatus.Error, moveArtifactResult.msg);
                            return moveArtifactResult.status;
                        }
                        break;

                    case MoveArtifactsFlowTable moveArtifacts:

                        var moveArtifactsResult = await IMissionTableOp.UpsertFlow(moveArtifacts);

                        if (!moveArtifactsResult.status)
                        {
                            await IObserverLib.NotifyNLog(EStatus.Error, moveArtifactsResult.msg);
                            return moveArtifactsResult.status;
                        }
                        break;

                    case RobotWinderFlowTable robotWinder:

                        var robotWinderResult = await IMissionTableOp.UpsertFlow(robotWinder);

                        if (!robotWinderResult.status)
                        {
                            await IObserverLib.NotifyNLog(EStatus.Error, robotWinderResult.msg);
                            return robotWinderResult.status;
                        }
                        break;
                }
            }

            return true;
        }

    }

    public delegate Task dgFarRobotMissionUpdated(List<AmrMissionTable> missions);

    public delegate Task dgFarRobotMissionParamUpdated(List<string> flowNames, List<string> cellNames,
                                                       Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts,
                                                       List<ArtifactInformDto> extArtifacts);

    public partial class FarRobotMissionApplication : IMissionObserver
    {
        public dgFarRobotMissionUpdated dgFarRobotMissionUpdated;

        public dgFarRobotMissionParamUpdated dgFarRobotMissionParamUpdated;

        public async Task HandleMissionUpdated(List<AmrMissionTable> list)
        {
            dgFarRobotMissionUpdated?.Invoke(list);
        }

        public async Task HandleMissionParamUpdated(List<string> flowNames, List<string> cellNames, 
                                                    Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts, 
                                                    List<ArtifactInformDto> extArtifacts)
        {
            dgFarRobotMissionParamUpdated?.Invoke(flowNames, cellNames, amrEmbArtifacts, extArtifacts);
        }
    }
}
