using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        public ObserverLibrary observerLibrary;

        void _createProjectLibrary()
        {
            observerLibrary = provider.GetRequiredService<ObserverLibrary>();
        }

        void _initProjectLibrary()
        {
            observerLibrary.AddNLogWritterObserver(logger);
        }
    }
}
