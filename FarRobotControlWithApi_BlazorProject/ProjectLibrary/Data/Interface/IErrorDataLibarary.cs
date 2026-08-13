namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{
    public interface IErrorDataLibarary
    {
        Task NotifySysDisconect();

        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);
    }
}
