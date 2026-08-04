using CommonLibraryB.Manager.WebApiClient;
using CommonLibraryB.Tools.LogWritter;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        public WebApiClientManager webApiClientManager;

        void _createManager()
        {
            webApiClientManager = provider.GetRequiredService<WebApiClientManager>();
        }

        void _initManager()
        {
            string logApi = string.Empty;

            if(!webApiClientManager.Connect(out logApi))
            {
                observerLibrary.NotifyNLog(EStatus.Error, logApi);
            }
        }
    }
}
