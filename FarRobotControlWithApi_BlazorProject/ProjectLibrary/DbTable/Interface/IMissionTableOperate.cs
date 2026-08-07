using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface
{
    public interface IMissionTableOperate
    {
        List<AmrMissionTable> listAmrMissionInQueue { get; set; }
    }
}
