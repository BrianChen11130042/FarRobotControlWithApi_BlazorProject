namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission.Interface
{
    public interface ISwarmCoreSetMissionTaskPack
    {
        Task<bool> GetNextMission();

        bool IsNewMission();

        bool IsCancelMissionBeforeDispatch();

        bool IsCancelMissionAfterDispatch();


        Task<bool> DispatchNewMission();

        Task<bool> UpsertMissionTable();

        Task NotifyMissionUpdated();
    }
}
