namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public enum EMissionState
    {
        DISPATCH_REQUEST,
        CANCEL_REQUEST,
        RETRY_REQUEST,
        CONTINUE_REQUEST,

        RUNNING,
        FAILED,
        COMPLETED,
        CANCELED
    }
}
