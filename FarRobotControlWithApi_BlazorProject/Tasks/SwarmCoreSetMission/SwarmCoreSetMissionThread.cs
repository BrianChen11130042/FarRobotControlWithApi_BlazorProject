using CommonLibraryB.Base.FiniteStateMachine;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreSetMission
{
    public partial class SwarmCoreSetMissionThread
    {
        readonly SwarmCoreSetMissionTask setMissionTask;

        public SwarmCoreSetMissionThread(SwarmCoreSetMissionTask setMissionTask)
        {
            this.setMissionTask = setMissionTask;

            interval = 1;
        }

    }

    public enum ESetMissionThread
    {
        None,
        SetMission
    }

    public partial class SwarmCoreSetMissionThread : FSMBase<ESetMissionThread, int>
    {
        public async override Task Init()
        {
            setMissionTask.Set(ES1.Action, ESetMission.CheckMission, 0);

            Set(ES1.Action, ESetMissionThread.SetMission, 0);
        }

        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case ESetMissionThread.None:
                    Set(ES1.Finish, ESetMissionThread.None, 0);
                    break;

                case ESetMissionThread.SetMission:
                    switch(S3)
                    {
                        case 0:
                            await setMissionTask.Run();

                            if(setMissionTask.key == EHandshakeKey.Finish)
                            {
                                if(setMissionTask.isError)
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
