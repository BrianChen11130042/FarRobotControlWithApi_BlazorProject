using DevExpress.Blazor;

namespace FarRobotControlWithApi_BlazorProject.UITools.ToastMessage
{
    public delegate Task dgNotifyToastMsg(string title, string msg, ToastRenderStyle style);

    public class ToastMessage : IToastMessage
    {
        public event dgNotifyToastMsg dgNotifyToast;

        public void NotifyInfo(string msg)
        {
            dgNotifyToast?.Invoke("系統訊息", msg, ToastRenderStyle.Info);
        }

        public void NotifySuccess(string msg)
        {
            dgNotifyToast?.Invoke("系統訊息", msg, ToastRenderStyle.Success);
        }

        public void NotifyError(string msg)
        {
            dgNotifyToast?.Invoke("系統訊息", msg, ToastRenderStyle.Danger);
        }
    }
}
