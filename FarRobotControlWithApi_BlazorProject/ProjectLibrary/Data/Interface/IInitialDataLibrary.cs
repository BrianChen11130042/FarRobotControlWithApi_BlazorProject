using FarRobotControlWithApi_BlazorProject.DTOModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface
{
    public interface IInitialDataLibrary
    {
        Task<bool> InitAmrMissionInQueue();

        Dictionary<string, List<ArtifactInformDto>> DcAmrArtifactMap { get; set; }

        List<string> ListFlowName { get; set; }

        List<string> ListCellName { get; set; }

        Task NotifyMissionUpdated();

        Task NotifyMissionParamUpdated();

        Task NotifyIntialResult(bool success, string msg);

        Task WriteNLogError(string log);

        Task WriteNLogInform(string log);
    }
}
