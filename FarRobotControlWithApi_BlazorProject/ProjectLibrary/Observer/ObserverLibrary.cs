using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Observer
{
    public partial class ObserverLibrary : INLogObservable
    {
        List<INLogObserver> os { get; set; }

        public void AddNLogObserver(INLogObserver o)
        {
            if (os == null)
                os = new List<INLogObserver>();

            if (!os.Contains(o))
            {
                os.Add(o);
            }
        }

        public void RemoveNLogObserver(INLogObserver o)
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
                    await o.HandleNLog(status, msg);
                }
            }
        }
    }

    public partial class ObserverLibrary : IMissionObservable
    {
        List<IMissionObserver> osMission { get; set; }

        public void AddMissionObserver(IMissionObserver o)
        {
            if(osMission == null)
                osMission = new List<IMissionObserver>();

            if(!osMission.Contains(o))
            {
                osMission.Add(o);
            }
        }

        public void RemoveMissionObserver(IMissionObserver o)
        {
            if(osMission != null && osMission.Contains(o))
            {
                osMission.Remove(o);
            }
        }

        public async Task NotifyMissionUpdated(List<AmrMissionTable> list)
        {
            if(osMission != null)
            {
                foreach(var o in osMission)
                {
                    await o.HandleMissionUpdated(list);
                }
            }
        }

        public async Task NotifyMissionParamUpdated(List<string> flowNames, List<string> amrIds, List<string> cellNames)
        {
            if(osMission != null)
            {
                foreach (var o in osMission)
                {
                    await o.HandleMissionParamUpdated(flowNames, amrIds, cellNames);
                }
            }
        }
    }

    public partial class ObserverLibrary : ISystemControlObservable
    {
        List<ISystemControlObserver> osSysControl { get; set; }

        public void AddSystemControlObserver(ISystemControlObserver o)
        {
            if(osSysControl == null)
                osSysControl = new List<ISystemControlObserver>();

            if(!osSysControl.Contains(o))
            {
                osSysControl.Add(o);
            }
        }

        public void RemoveSystemControlObserver(ISystemControlObserver o)
        {
            if(osSysControl != null && osSysControl.Contains(o))
            {
                osSysControl.Remove(o);
            }
        }

        public async Task NotifyDisconnect()
        {
            if(osSysControl != null)
            {
                foreach(var o in osSysControl)
                {
                    await o.HandleDisconnect();
                }
            }
        }

        public async Task NotifyInitialResult(bool success, string msg)
        {
            if(osSysControl != null)
            {
                foreach(var o in osSysControl)
                {
                    await o.HandleInitialResult(success, msg);
                }
            }
        }
    }
}
