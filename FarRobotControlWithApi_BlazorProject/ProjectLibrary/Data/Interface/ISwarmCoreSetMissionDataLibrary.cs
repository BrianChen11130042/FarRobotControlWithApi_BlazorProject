namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{

    public interface ISwarmCoreSetMissionDataLibrary
    {
        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);
    }
}
