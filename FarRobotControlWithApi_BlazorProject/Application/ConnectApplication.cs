using CommonLibraryB.Library.AmrControl.Config;
using CommonLibraryB.Manager.WebApiClient;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;

namespace FarRobotControlWithApi_BlazorProject.Application
{
    public partial class ConnectApplication
    {
        WebApiClientManager webApiClientManager;
        AmrControlConfigManager<EAmrControl> amrControlConfig;

        public ConnectApplication(WebApiClientManager webApiClientManager,
                                  AmrControlConfigManager<EAmrControl> amrControlConfig)
        {
            this.webApiClientManager = webApiClientManager;
            this.amrControlConfig = amrControlConfig;
        }
    }

    public delegate Task dgInitialResult(bool success, string msg);

    public partial class ConnectApplication : ISystemControlObserver
    {
        public async Task<List<WebApiClientConfig>> GetWebApiClientConfig()
        {
            List<WebApiClientConfig> list = new List<WebApiClientConfig>();

            foreach (string dev in Enum.GetNames(typeof(EWebApiClient)))
            {
                WebApiClientConfig config = webApiClientManager.Get(dev);

                if (config != null)
                {
                    list.Add(config);
                }
            }

            return list;
        }

        public async Task SetWebApiClientConfig(WebApiClientConfig config)
        {
            webApiClientManager.Set(config.device, config);
            webApiClientManager.Save();
        }

        public async Task<List<AmrControlConfig>> GetAmrControlConfig()
        {
            List<AmrControlConfig> list = new List<AmrControlConfig>();

            foreach (string dev in Enum.GetNames(typeof(EAmrControl)))
            {
                AmrControlConfig config = amrControlConfig.Get(dev);

                if (config != null)
                {
                    list.Add(config);
                }
            }

            return list;
        }

        public async Task SetAmrControlConfig(AmrControlConfig config)
        {
            amrControlConfig.Set(config.device, config);
            amrControlConfig.Save();
        }

        public dgInitialResult dgInitialResult;

        public async Task HandleInitialResult(bool success, string msg)
        {
            dgInitialResult?.Invoke(success, msg);
        }

        public async Task HandleDisconnect()
        {
            
        }
    }
}
