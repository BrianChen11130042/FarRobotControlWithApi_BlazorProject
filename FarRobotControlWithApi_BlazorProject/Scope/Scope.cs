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
            _createCommonService();
            _createManager();
            _createAmrControl();
        }

        public void initAll()
        {
            _initCommonService();
            _initManager();
            _initAmrControl();
        }
    }
}
