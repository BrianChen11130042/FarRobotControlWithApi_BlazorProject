using CommonLibraryB.Library.AmrControl.Config;
using CommonLibraryB.Manager.WebApiClient;
using FarRobotControlWithApi_BlazorProject.DTOModel;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.Scope;
using FarRobotControlWithApi_BlazorProject.Services.Interface;

namespace FarRobotControlWithApi_BlazorProject.Services
{
    public partial class FarRobotMachineService : IFarRobotMachineService
    {
        readonly MachineScope scope;

        public FarRobotMachineService(MachineScope scope)
        {
            this.scope = scope;

            scope.connectApp.dgInitialResult += InitialResult;

            scope.farRobotMissionApp.dgFarRobotMissionUpdated += FarRobotMissionUpdated;
            scope.farRobotMissionApp.dgFarRobotMissionParamUpdated += FarRobotMissionParamUpdated;
        }
    }

    public delegate Task dgInitResult(bool success, string msg);

    public partial class FarRobotMachineService
    {
        public async Task<List<WebApiClientConfig>> GetWebApiClientConfig()
        {
            return await scope.connectApp.GetWebApiClientConfig();
        }

        public async Task SetWebApiClientConfig(WebApiClientConfig config)
        {
            await scope.connectApp.SetWebApiClientConfig(config);
        }

        public async Task<List<AmrControlConfig>> GetAmrControlConfig()
        {
            return await scope.connectApp.GetAmrControlConfig();
        }

        public async Task SetAmrControlConfig(AmrControlConfig config)
        {
            await scope.connectApp.SetAmrControlConfig(config);
        }

        public async Task Initial()
        {
            scope.initAll();
        }

        public event dgInitResult dgInitResult;

        public async Task InitialResult(bool success, string msg)
        {
            dgInitResult?.Invoke(success, msg);
        }
    }

    public delegate Task dgAmrMissionUpdated(List<AmrMissionTable> missions);
    public delegate Task dgAmrMissionParamUpdated(List<string> flowNames, List<string> cellNames, 
                                             Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts,
                                             List<ArtifactInformDto> extArtifacts);

    public partial class FarRobotMachineService
    {
        public event dgAmrMissionUpdated dgAmrMissionUpdate;
        public event dgAmrMissionParamUpdated dgAmrMissionParamUpdate;

        public async Task FarRobotMissionUpdated(List<AmrMissionTable> list)
        {
            dgAmrMissionUpdate?.Invoke(list);
        }

        public async Task FarRobotMissionParamUpdated(List<string> flowNames, List<string> cellNames, 
                                                      Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts, 
                                                      List<ArtifactInformDto> extArtifacts)
        {
            dgAmrMissionParamUpdate?.Invoke(flowNames, cellNames, amrEmbArtifacts, extArtifacts);
        }

        public async Task<List<AmrMissionTable>> GetAmrMissionInQueue()
        {
            return await scope.farRobotMissionApp.GetAmrMissionInQueue();
        }

        public async Task<(List<string> flowNames, List<string> cellNames, 
                           Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts, 
                           List<ArtifactInformDto> extArtifacts)> GetAmrMissionParam()
        {
            return await scope.farRobotMissionApp.GetAmrMissionParam();
        }

        public async Task<bool> SetMission(AmrMissionTable mission)
        {
            return await scope.farRobotMissionApp.SetMission(mission);
        }

        public async Task<bool> CancelMission(Guid missionId)
        {
            return await scope.farRobotMissionApp.CancelMission(missionId);
        }

        public async Task<bool> RetryMission(Guid missionId)
        {
            return await scope.farRobotMissionApp.RetryMission(missionId);
        }

        public async Task<bool> ContinueMission(Guid missionId)
        {
            return await scope.farRobotMissionApp.ContinueMission(missionId);
        }
    }
}
