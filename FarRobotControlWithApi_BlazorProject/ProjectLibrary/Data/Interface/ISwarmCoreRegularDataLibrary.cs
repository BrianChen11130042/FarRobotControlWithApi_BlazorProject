using FarRobotControlWithApi_BlazorProject.DTOModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{
    public interface ISwarmCoreRegularDataLibrary
    {
        AccessTokenDto TokenInform { get; set; }

        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);

    }
}
