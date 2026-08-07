using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface
{
    public interface ILogTableOperate
    {
        List<LogTable> listLogData { get; set; }

        Task<(bool status, string msg)> AddLogData(LogTable data);
    }
}
