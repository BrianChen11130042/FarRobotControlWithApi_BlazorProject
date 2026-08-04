using DevExpress.Blazor;

namespace FarRobotControlWithApi_BlazorProject.UITools.ToastMessage
{
    public class ToastMessage
    {
        readonly IToastNotificationService toast;

        public ToastMessage(IToastNotificationService toast)
        {
            this.toast = toast;
        }

        public void NotifyInfo(string msg)
        {
            Show("系統訊息", msg, ToastRenderStyle.Info);
        }

        public void NotifySuccess(string msg)
        {
            Show("系統訊息", msg, ToastRenderStyle.Success);
        }

        public void NotifyError(string msg)
        {
            Show("系統訊息", msg, ToastRenderStyle.Danger);
        }

        void Show(string title, string msg, ToastRenderStyle style)
        {
            toast.ShowToast(new ToastOptions
            {
                Title = title,
                Text = msg,
                RenderStyle = style
            });
        }
    }
}
