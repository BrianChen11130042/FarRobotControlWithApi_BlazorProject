using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data;
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

        public SwarmCoreRegularDataLibary swarmCoreRegularDataLibary;
        public SwarmCoreSetMissionDataLibrary swarmCoreSetMissionDataLibrary;
        public InitialDataLibrary initialDataLibrary;
        public ErrorDataLibrary errorDataLibrary;

        void _createProjectLibrary()
        {
            observerLibrary = provider.GetRequiredService<ObserverLibrary>();

            missionTableLibrary = provider.GetRequiredService<MissionTableLibrary>();
            logTableLibrary = provider.GetRequiredService<LogTableLibrary>();

            swarmCoreRegularDataLibary = new SwarmCoreRegularDataLibary(observerLibrary);
            swarmCoreSetMissionDataLibrary = new SwarmCoreSetMissionDataLibrary(logTableLibrary, 
                                                                                missionTableLibrary,
                                                                                observerLibrary, 
                                                                                observerLibrary);
            initialDataLibrary = new InitialDataLibrary(logTableLibrary,
                                                        missionTableLibrary,
                                                        observerLibrary,
                                                        observerLibrary,
                                                        observerLibrary);

            errorDataLibrary = new ErrorDataLibrary(logTableLibrary,
                                                    observerLibrary,
                                                    observerLibrary);
        }

        void _initProjectLibrary()
        {
            observerLibrary.AddNLogObserver(logger);
        }
    }
}
