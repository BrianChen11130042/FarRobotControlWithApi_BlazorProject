using FarRobotControlWithApi_BlazorProject.DTOModel;
using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface
{
    public interface IMissionObservable
    {
        void AddMissionObserver(IMissionObserver o);

        void RemoveMissionObserver(IMissionObserver o);

        Task NotifyMissionUpdated(List<AmrMissionTable> list);

        Task NotifyMissionParamUpdated(List<string> flowNames, List<string> cellNames, 
                                       Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts, 
                                       List<ArtifactInformDto> extArtifacts);
    }

    public interface IMissionObserver
    {
        Task HandleMissionUpdated(List<AmrMissionTable> list);

        Task HandleMissionParamUpdated(List<string> flowNames, List<string> cellNames, 
                                       Dictionary<string, List<ArtifactInformDto>> amrEmbArtifacts, 
                                       List<ArtifactInformDto> extArtifacts);
    }
}
