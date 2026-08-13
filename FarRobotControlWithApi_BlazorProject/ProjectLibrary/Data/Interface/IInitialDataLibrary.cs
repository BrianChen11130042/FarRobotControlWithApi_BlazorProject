namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{
    public interface IInitialDataLibrary
    {
        Task<bool> InitAmrMissionInQueue();

        Task NotifyMissionUpdated();

        Task NotifyIntialResult(bool success, string msg);

        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);
    }
}
