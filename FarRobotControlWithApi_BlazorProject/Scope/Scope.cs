namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        IServiceProvider provider;

        public MachineScope(IServiceProvider provider)
        {
            this.provider = provider;
            _createAll();
        }

        void _createAll()
        {
            _createTool();
            _createProjectLibrary();
            _createManager();
            _createAmrControl();
        }

        public void initAll()
        {
            _stopThread();

            _initProjectLibrary();
            _initManager();
            _initAmrControl();

            _initSysControlTask();
            _initSetMissionTask();
            _initRegularTask();
            _initThreadTask();
            _initThread();
        }
    }
}
