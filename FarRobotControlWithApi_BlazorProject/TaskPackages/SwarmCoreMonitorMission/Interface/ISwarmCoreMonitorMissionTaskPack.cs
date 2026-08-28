namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreMonitorMission.Interface
{
    public interface ISwarmCoreMonitorMissionTaskPack
    {
        Task<bool> GetRunningMissionList();

        bool IsGetRunningMissionList();

        bool IsNextRunningMissionTarget();

        Task<bool> GetRunningMissionTarget();

        Task<bool> GetProgressByFlowId();

        bool IsNeedGetArtifactStatus();

        Task<bool> GetArtifactStatusByArtifactId();

        Task<bool> UpsertMissionTable();

        Task NotifyMissionUpdated();
    }
}
