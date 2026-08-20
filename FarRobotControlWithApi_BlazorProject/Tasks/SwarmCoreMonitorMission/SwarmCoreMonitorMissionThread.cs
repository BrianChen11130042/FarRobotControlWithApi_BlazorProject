using CommonLibraryB.Base.FiniteStateMachine;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreMonitorMission
{
    public partial class SwarmCoreMonitorMissionThread
    {
        readonly SwarmCoreMonitorMissionTask monitorMissionTask;

        public SwarmCoreMonitorMissionThread(SwarmCoreMonitorMissionTask monitorMissionTask)
        {
            this.monitorMissionTask = monitorMissionTask;

            interval = 1;
        }
    }

    public enum EMonitorMissionThread
    {
        None,
        MonitorMission
    }

    public partial class SwarmCoreMonitorMissionThread : FSMBase<EMonitorMissionThread, int>
    {
        public async override Task Init()
        {
            monitorMissionTask.Set(ES1.Action, EMonitorMission.CheckStartedMission, 0);

            Set(ES1.Action, EMonitorMissionThread.MonitorMission, 0);
        }

        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case EMonitorMissionThread.None:
                    Set(ES1.Finish, EMonitorMissionThread.None, 0);
                    break;

                case EMonitorMissionThread.MonitorMission:
                    switch(S3)
                    {
                        case 0:
                            await monitorMissionTask.Run();

                            if (monitorMissionTask.key == EHandshakeKey.Finish)
                            {
                                if (monitorMissionTask.isError)
                                {
                                    SaveState();

                                    isError = true;
                                }
                            }

                            Set(0);
                            break;
                    }
                    break;
            }
        }

        public async override Task Error()
        {
            //throw new NotImplementedException();
        }

        public async override Task Finish()
        {
            //throw new NotImplementedException();
        }

        public async override Task Idle()
        {
            //throw new NotImplementedException();
        }
    }
}
