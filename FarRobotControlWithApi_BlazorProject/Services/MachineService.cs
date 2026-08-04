using CommonLibraryB.Library.AmrControl.Config;
using CommonLibraryB.Manager.WebApiClient;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
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
}
