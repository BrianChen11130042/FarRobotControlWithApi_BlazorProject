namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Initial.Interface
{
    public interface IInitialTaskPack
    {
        Task<bool> InitAmrMissionInQueue();

        Task<bool> InitSwarmCore();

        Task NotifyMissionUpdated();

        Task NotifyMissionParamUpdated();

        Task NotifyInitialSuccess();

        Task NotifyInitialFail();
    }
}
