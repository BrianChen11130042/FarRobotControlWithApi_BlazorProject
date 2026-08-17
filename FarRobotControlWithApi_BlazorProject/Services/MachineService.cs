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
    public partial class MachineService : IMachineService
    {
        public MachineScope scope;

        public MachineService(MachineScope scope)
        {
            this.scope = scope;

            scope.observerLibrary.AddMissionObserver(this);
        }
    }

    public partial class MachineService
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
    }

    public delegate Task dgAmrMissionUpdated(List<AmrMissionTable> missions);

    public partial class MachineService : IMissionObserver
    {
        public event dgAmrMissionUpdated dgAmrMissionUpdate;

        public async Task HandleMissionUpdated(List<AmrMissionTable> list)
        {
            dgAmrMissionUpdate?.Invoke(list);
        }

        public async Task<List<AmrMissionTable>> GetAmrMissionInQueue()
        {
            return scope.missionTableLibrary.listAmrMissionInQueue;
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
    }
}
