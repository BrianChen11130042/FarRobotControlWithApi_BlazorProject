namespace FarRobotControlWithApi_BlazorProject.UITools.ToastMessage
{
    public interface IToastMessage
    {
        void NotifyInfo(string msg);

        void NotifySuccess(string msg);

        void NotifyError(string msg);

        event dgNotifyToastMsg dgNotifyToast;
    }
}
