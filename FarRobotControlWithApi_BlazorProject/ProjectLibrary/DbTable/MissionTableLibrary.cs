using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable
{
    public partial class MissionTableLibrary
    {
        readonly IServiceProvider serviceProvider;

        public MissionTableLibrary(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        List<AmrMissionTable> _listAmrMissionInQueue { get; set; } = new List<AmrMissionTable>();

        public List<AmrMissionTable> listAmrMissionInQueue
        {
            get
            {
                return _listAmrMissionInQueue;
            }
            set
            {
                _listAmrMissionInQueue = value;
            }
        }
    }

    public partial class MissionTableLibrary : IMissionTableOperate
    {

    }
}
