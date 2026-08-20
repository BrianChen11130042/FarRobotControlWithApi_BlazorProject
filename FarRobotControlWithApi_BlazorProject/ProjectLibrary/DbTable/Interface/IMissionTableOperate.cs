using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface
{
    public interface IMissionTableOperate
    {
        List<AmrMissionTable> listAmrMissionInQueue { get; set; }

        Task<(bool status, string msg)> InitAmrMissionInQueue();

        Task<AmrMissionTable> GetNextMissionTable();

        Task<List<AmrMissionTable>> GetStartedMissionTableList();

        Task<(bool status, string msg)> UpsertMissionTable(AmrMissionTable data);

        Task<(bool status, string msg)> UpsertFlow<T>(T data) where T : FlowBase;
    }
}
