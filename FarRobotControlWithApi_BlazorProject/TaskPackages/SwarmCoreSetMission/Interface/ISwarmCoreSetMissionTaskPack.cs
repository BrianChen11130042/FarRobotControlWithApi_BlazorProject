namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission.Interface
{
    public interface ISwarmCoreSetMissionTaskPack
    {
        Task<bool> GetNextMission();

        bool IsNewMission();

        bool IsCancelMission();


        Task<bool> DispatchNewMission();

        Task<bool> DispatchCancelMission();

        Task<bool> UpsertMissionTable();

        Task NotifyMissionUpdated();
    }
}
