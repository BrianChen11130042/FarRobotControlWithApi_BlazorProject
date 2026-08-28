using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{
    public interface ISwarmCoreMonitorMissionDataLibrary
    {
        List<AmrMissionTable> ListRunningMission { get; set; }

        AmrMissionTable TargetRunningMission { get; set; }

        int StartedIndex { get; set; }

        Task<bool> GetRunningMissionTableList();

        Task<bool> GetRunningMissionTableTarget();

        Task<bool> UpsertMissionTable();


        Task NotifyMissionUpdated();

        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);
    }
}
