using FarRobotControlWithApi_BlazorProject.Application;

namespace FarRobotControlWithApi_BlazorProject.Scope
{

    public partial class MachineScope
    {
        public ConnectApplication connectApp;
        public FarRobotMissionApplication farRobotMissionApp;

        void _createApplication()
        {
            connectApp = new ConnectApplication(webApiClientManager, amrControlConfig);

            farRobotMissionApp = new FarRobotMissionApplication(missionTableLibrary,
                                                                initialDataLibrary,
                                                                observerLibrary);
        }

        void _initApplication()
        {
            observerLibrary.AddSystemControlObserver(connectApp);
            observerLibrary.AddMissionObserver(farRobotMissionApp);
        }
    }
}
