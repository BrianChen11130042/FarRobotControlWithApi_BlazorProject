using FarRobotControlWithApi_BlazorProject.EFModel;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface
{
    public interface IMissionObservable
    {
        void AddMissionObserver(IMissionObserver o);

        void RemoveMissionObserver(IMissionObserver o);

        Task NotifyMissionUpdated(List<AmrMissionTable> list);

    }

    public interface IMissionObserver
    {
        Task HandleMissionUpdated(List<AmrMissionTable> list);
    }
}
