namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreRegular.Interface
{
    public interface ISwarmCoreRegularTaskPack
    {
        bool IsGetAccessToken();

        Task<bool> GetAccessToken();
    }
}
