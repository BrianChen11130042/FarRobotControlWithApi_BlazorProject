using CommonLibraryB.Base.FiniteStateMachine;
using DevExpress.Utils.Filtering.Internal;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreRegular;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreSetMission;
using FarRobotControlWithApi_BlazorProject.Tasks.SystemControl;

namespace FarRobotControlWithApi_BlazorProject.Tasks.Main
{
    public partial class MainThread
    {

        readonly SystemControlThread controlThread;
        readonly SwarmCoreRegularThread regularThread;
        readonly SwarmCoreSetMissionThread setMissionThread;

        public MainThread(SystemControlThread controlThread, 
                          SwarmCoreRegularThread regularThread, 
                          SwarmCoreSetMissionThread setMissionThread)
        {
            this.controlThread = controlThread;
            this.regularThread = regularThread;
            this.setMissionThread = setMissionThread;

            interval = 1;
        }
    }

    public enum EMainThread
    {
        None,
        Monitor
    }

    public partial class MainThread : FSMBase<EMainThread, int>
    {
        public async override Task Init()
        {
            switch(S3)
            {
                case 0:
                    controlThread.ResetKey();
                    controlThread.Set(ES1.Action, ESystemControlThread.InitialTask, 0);

                    Set(10);
                    break;

                case 10:
                    if(controlThread.key == EHandshakeKey.Finish)
                    {
                        if(controlThread.isError)
                        {
                            Set(ES1.None, EMainThread.None, 0);
                        }
                        else
                        {
                            regularThread.ResetKey();
                            setMissionThread.ResetKey();

                            regularThread.Set(ES1.Init, ESwarmCoreRegularThread.None, 0);
                            setMissionThread.Set(ES1.Init, ESetMissionThread.None, 0);

                            Set(ES1.Action, EMainThread.Monitor, 0);
                        }
                    }
                    else
                    {
                        Set(10);
                    }
                    break;
            }
        }

        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case EMainThread.None:
                    Set(ES1.Finish, EMainThread.None, 0);
                    break;

                case EMainThread.Monitor:
                    switch(S3)
                    {
                        case 0:
                            if(regularThread.isError)
                            {
                                SaveState();
                                Set(ES1.Error, EMainThread.None, 0);
                            }
                            else
                            {
                                Set(10);
                            }
                            break;

                        case 10:
                            if(setMissionThread.isError)
                            {
                                SaveState();
                                Set(ES1.Error, EMainThread.None, 0);
                            }
                            else
                            {
                                Set(0);
                            }
                            break;
                    }
                    break;
            }
        }

        public async override Task Error()
        {
            switch(S3)
            {
                case 0:
                    controlThread.ResetKey();
                    controlThread.Set(ES1.Action, ESystemControlThread.ErrorTask, 0);

                    Set(10);
                    break;

                case 10:
                    if(controlThread.key == EHandshakeKey.Finish)
                    {
                        Set(ES1.Idle, EMainThread.None, 0);
                    }
                    else
                    {
                        Set(10);
                    }
                    break;
            }
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
