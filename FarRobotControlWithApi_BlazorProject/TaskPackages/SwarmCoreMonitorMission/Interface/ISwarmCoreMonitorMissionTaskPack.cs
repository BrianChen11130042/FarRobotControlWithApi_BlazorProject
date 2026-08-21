namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreMonitorMission.Interface
{
    public interface ISwarmCoreMonitorMissionTaskPack
    {
        Task<bool> GetStartedMissionList();

        bool IsGetStartedMissionList();

        bool IsNextStartedMissionTarget();

        Task<bool> GetStartedMissionTarget();

        Task<bool> GetProgressByFlowId();

        bool IsNeedGetArtifactStatus();

        Task<bool> GetArtifactStatusByArtifactId();

        Task<bool> UpsertMissionTable();

        Task NotifyMissionUpdated();
    }
}
