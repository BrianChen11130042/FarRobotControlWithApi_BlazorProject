using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer;
using Microsoft.Extensions.DependencyInjection;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        public ObserverLibrary observerLibrary;

        public MissionTableLibrary missionTableLibrary;
        public LogTableLibrary logTableLibrary;

        void _createProjectLibrary()
        {
            observerLibrary = provider.GetRequiredService<ObserverLibrary>();

            missionTableLibrary = provider.GetRequiredService<MissionTableLibrary>();
            logTableLibrary = provider.GetRequiredService<LogTableLibrary>();
        }

        void _initProjectLibrary()
        {
            observerLibrary.AddNLogWritterObserver(logger);
        }
    }
}
