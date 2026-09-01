namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission.Interface
{
    public interface ISwarmCoreSetMissionTaskPack
    {
        Task<bool> GetNextMission();

        bool IsNewMission();

        bool IsCancelMission();

        bool IsRetryMission();

        bool IsContinueMission();

        Task<bool> DispatchNewMission();

        Task<bool> DispatchCancelMission();

        Task<bool> DispatchRetryMission();

        Task<bool> DispatchContinueMission();

        Task<bool> UpsertMissionTable();

        Task NotifyMissionUpdated();
    }
}
