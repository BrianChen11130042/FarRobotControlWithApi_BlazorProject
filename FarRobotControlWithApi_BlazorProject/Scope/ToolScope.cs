using CommonLibraryB.Tools.LogWritter;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {

        public LogWritter logger;

        void _createTool()
        {
            logger = provider.GetRequiredService<LogWritter>();
        }

    }
}
