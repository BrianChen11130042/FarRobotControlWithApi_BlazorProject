using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{

    public interface ISwarmCoreSetMissionDataLibrary
    {
        AmrMissionTable AmrMission { get; set; }

        Task<bool> GetNextMissionTable();

        Task<bool> UpsertMissionTable();


        Task NotifyMissionUpdated();

        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);
    }
}
