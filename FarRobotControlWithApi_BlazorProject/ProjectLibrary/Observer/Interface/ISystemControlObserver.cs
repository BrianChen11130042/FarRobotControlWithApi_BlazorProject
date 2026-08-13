namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface
{
    public interface ISystemControlObservable
    {
        void AddSystemControlObserver(ISystemControlObserver o);

        void RemoveSystemControlObserver(ISystemControlObserver o);

        Task NotifyInitialResult(bool success, string msg);

        Task NotifyDisconnect();
    }

    public interface ISystemControlObserver
    {
        Task HandleInitialResult(bool success, string msg);

        Task HandleDisconnect();
    }
}
