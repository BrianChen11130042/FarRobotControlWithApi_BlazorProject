using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{
    public interface ISwarmCoreMonitorMissionDataLibrary
    {
        List<AmrMissionTable> ListStartedMission { get; set; }

        AmrMissionTable TargetStartedMission { get; set; }

        int StartedIndex { get; set; }

        Task<bool> GetStartedMissionTableList();

        Task<bool> GetStartedMissionTableTarget();

        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);
    }
}
