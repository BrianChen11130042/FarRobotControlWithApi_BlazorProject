namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Initial.Interface
{
    public interface IInitialTaskPack
    {
        Task<bool> InitAmrMissionInQueue();

        Task<bool> CheckSwarmCoreConnect();

        Task NotifyMissionUpdated();

        Task NotifyInitialSuccess();

        Task NotifyInitialFail();
    }
}
