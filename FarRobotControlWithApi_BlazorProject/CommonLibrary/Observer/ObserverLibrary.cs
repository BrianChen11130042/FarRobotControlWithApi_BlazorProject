using CommonLibraryB.Tools.LogWritter;

namespace FarRobotControlWithApi_BlazorProject.CommonLibrary.Observer
{
    public partial class ObserverLibrary : INLogWritterObservable
    {
        List<INLogWritterObserver> os { get; set; }

        public void AddNLogWritterObserver(INLogWritterObserver o)
        {
            if (os == null)
                os = new List<INLogWritterObserver>();

            if (!os.Contains(o))
            {
                os.Add(o);
            }
        }

        public void RemoveNLogWritterObserver(INLogWritterObserver o)
        {
            if (os != null && os.Contains(o))
            {
                os.Remove(o);
            }
        }

        public async Task NotifyNLog(EStatus status, string msg)
        {
            if (os != null)
            {
                foreach (var o in os)
                {
                    await o.WriteNLog(status, msg);
                }
            }
        }
    }
}
