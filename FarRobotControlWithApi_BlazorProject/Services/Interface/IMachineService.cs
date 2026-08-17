using CommonLibraryB.Library.AmrControl.Config;
using CommonLibraryB.Manager.WebApiClient;
using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.Services.Interface
{
    public interface IMachineService
    {
        //連線初始化
        Task<List<WebApiClientConfig>> GetWebApiClientConfig();

        Task SetWebApiClientConfig(WebApiClientConfig config);

        Task<List<AmrControlConfig>> GetAmrControlConfig();

        Task SetAmrControlConfig(AmrControlConfig config);

        Task Initial();

        //任務
        Task<List<AmrMissionTable>> GetAmrMissionInQueue();

        Task<bool> SetMission(AmrMissionTable mission);

        event dgAmrMissionUpdated dgAmrMissionUpdate;

    }
}
