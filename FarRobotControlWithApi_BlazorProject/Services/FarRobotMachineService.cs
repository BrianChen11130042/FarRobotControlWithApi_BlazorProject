using CommonLibraryB.Library.AmrControl.Config;
using CommonLibraryB.Manager.WebApiClient;
using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;
using FarRobotControlWithApi_BlazorProject.Scope;
using FarRobotControlWithApi_BlazorProject.Services.Interface;

namespace FarRobotControlWithApi_BlazorProject.Services
{
    public partial class FarRobotMachineService : IFarRobotMachineService
    {
        public MachineScope scope;

        public FarRobotMachineService(MachineScope scope)
        {
            this.scope = scope;

            scope.observerLibrary.AddMissionObserver(this);
            scope.observerLibrary.AddSystemControlObserver(this);
        }
    }

    public delegate Task dgInitResult(bool success, string msg);

    public partial class FarRobotMachineService : ISystemControlObserver
    {
        public async Task<List<WebApiClientConfig>> GetWebApiClientConfig()
        {
            List<WebApiClientConfig> list = new List<WebApiClientConfig>();

            foreach(string dev in Enum.GetNames(typeof(EWebApiClient)))
            {
                WebApiClientConfig config = scope.webApiClientManager.Get(dev);

                if(config != null)
                {
                    list.Add(config);
                }
            }

            return list;
        }

        public async Task SetWebApiClientConfig(WebApiClientConfig config)
        {
            scope.webApiClientManager.Set(config.device, config);
            scope.webApiClientManager.Save();
        }

        public async Task<List<AmrControlConfig>> GetAmrControlConfig()
        {
            List<AmrControlConfig> list = new List<AmrControlConfig>();

            foreach(string dev in Enum.GetNames(typeof(EAmrControl)))
            {
                AmrControlConfig config = scope.amrControlConfig.Get(dev);

                if(config != null)
                {
                    list.Add(config);
                }
            }

            return list;
        }

        public async Task SetAmrControlConfig(AmrControlConfig config)
        {
            scope.amrControlConfig.Set(config.device, config);
            scope.amrControlConfig.Save();
        }

        public async Task Initial()
        {
            scope.initAll();
        }

        public event dgInitResult dgInitResult;

        public async Task HandleInitialResult(bool success, string msg)
        {
            dgInitResult?.Invoke(success, msg);
        }

        public async Task HandleDisconnect()
        {
            
        }
    }

    public delegate Task dgAmrMissionUpdated(List<AmrMissionTable> missions);
    public delegate Task dgAmrMissionParamUpdated(List<string> flowNames, List<string> amrIds, List<string> cellNames);

    public partial class FarRobotMachineService : IMissionObserver
    {
        public event dgAmrMissionUpdated dgAmrMissionUpdate;
        public event dgAmrMissionParamUpdated dgAmrMissionParamUpdate;

        public async Task HandleMissionUpdated(List<AmrMissionTable> list)
        {
            dgAmrMissionUpdate?.Invoke(list);
        }

        public async Task HandleMissionParamUpdated(List<string> flowNames, List<string> amrIds, List<string> cellNames)
        {
            dgAmrMissionParamUpdate?.Invoke(flowNames, amrIds, cellNames);
        }

        public async Task<List<AmrMissionTable>> GetAmrMissionInQueue()
        {
            return scope.missionTableLibrary.listAmrMissionInQueue;
        }

        public async Task<(List<string> flowNames, List<string> amrIds, List<string> cellNames)> GetAmrMissionParam()
        {
            List<string> flows = scope.initialDataLibrary.ListFlowName;
            List<string> amrs = scope.initialDataLibrary.ListAmrSerialNumber;
            List<string> cells = scope.initialDataLibrary.ListCellName;

            return (flows, amrs, cells);
        }

        public async Task<bool> SetMission(AmrMissionTable mission)
        {
            var result = await scope.missionTableLibrary.UpsertMissionTable(mission);

            if(result.status)
            {
                return result.status;
            }
            else
            {
                await scope.observerLibrary.NotifyNLog(EStatus.Error, result.msg);
                return result.status;
            }
        }

        public async Task<bool> CancelMission(Guid missionId)
        {

            AmrMissionTable? mission = scope.missionTableLibrary.listAmrMissionInQueue.FirstOrDefault(x => x.Id == missionId
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

            if(mission == null)
            {
                await scope.observerLibrary.NotifyNLog(EStatus.Error, "Mission not found in queue");
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
            AmrMissionTable? mission = scope.missionTableLibrary.listAmrMissionInQueue.FirstOrDefault(x => x.Id == missionId
                                                                                                        && x.IsFinish == false
                                                                                                        && x.IsCancel == false
                                                                                                        && x.Flows.Any(f => f.IsError && !f.IsFinish && !f.IsCancel)
                                                                                                        && string.Equals(x.MissionState,
                                                                                                                         EMissionState.FAILED.ToString(),
                                                                                                                         StringComparison.OrdinalIgnoreCase));

            if (mission == null)
            {
                await scope.observerLibrary.NotifyNLog(EStatus.Error, "Mission not found in queue");
                return false;
            }

            List<FlowBase> listFailFlow = mission.Flows.Where(f => f.IsError && !f.IsFinish && !f.IsCancel)
                                                       .OrderBy(f => f.EstablishTime)
                                                       .ToList();

            List<FlowBase> listRetryFlow = _getListRetryFlow(listFailFlow);

            if(!await _setListRetryFlow(listRetryFlow))
            {
                return false;
            }

            mission.FlowCount = mission.FlowCount + listRetryFlow.Count;
            mission.MissionState = EMissionState.RETRY_REQUEST.ToString();

            if(!await SetMission(mission))
            {
                return false;
            }

            return true;
        }

        List<FlowBase> _getListRetryFlow(List<FlowBase> listFailFlow)
        {
            DateTime now = DateTime.Now;
            List<FlowBase> listRetryFlow = new List<FlowBase>();

            foreach(FlowBase failFlow in listFailFlow)
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

                        var moveResult = await scope.missionTableLibrary.UpsertFlow(move);

                        if (!moveResult.status)
                        {
                            await scope.observerLibrary.NotifyNLog(EStatus.Error, moveResult.msg);
                            return moveResult.status;
                        }
                        break;
                    case ChargeFlowTable charge:

                        var chargeResult = await scope.missionTableLibrary.UpsertFlow(charge);

                        if (!chargeResult.status)
                        {
                            await scope.observerLibrary.NotifyNLog(EStatus.Error, chargeResult.msg);
                            return chargeResult.status;
                        }
                        break;
                }
            }

            return true;
        }

        public async Task<bool> ContinueMission(Guid missionId)
        {
            AmrMissionTable? mission = scope.missionTableLibrary.listAmrMissionInQueue.FirstOrDefault(x => x.Id == missionId
                                                                                                        && x.IsFinish == false
                                                                                                        && x.IsCancel == false
                                                                                                        && string.Equals(x.MissionState, 
                                                                                                                         EMissionState.FAILED.ToString(),
                                                                                                                         StringComparison.OrdinalIgnoreCase)
                                                                                                        && x.Flows.Any(f => f.IsStart && f.IsError && !f.IsFinish && !f.IsCancel));

            if (mission == null)
            {
                await scope.observerLibrary.NotifyNLog(EStatus.Error, "Mission not found in queue");
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
}
