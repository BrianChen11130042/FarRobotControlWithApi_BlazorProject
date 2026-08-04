using FarRobotControlWithApi_BlazorProject.CommonLibrary.Observer;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        public ObserverLibrary observerLibrary;

        void _createCommonService()
        {
            observerLibrary = provider.GetRequiredService<ObserverLibrary>();
        }

        void _initCommonService()
        {
            observerLibrary.AddNLogWritterObserver(logger);
        }
    }
}
