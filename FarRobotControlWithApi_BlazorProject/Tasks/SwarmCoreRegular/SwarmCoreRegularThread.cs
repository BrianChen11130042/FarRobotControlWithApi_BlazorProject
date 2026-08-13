using CommonLibraryB.Base.FiniteStateMachine;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreRegular
{

    public partial class SwarmCoreRegularThread
    {
        readonly SwarmCoreRegularTask SwarmCoreRegularTask;

        public SwarmCoreRegularThread(SwarmCoreRegularTask SwarmCoreRegularTask)
        {
            this.SwarmCoreRegularTask = SwarmCoreRegularTask;

            interval = 1;
        }
    }

    public enum ESwarmCoreRegularThread
    {
        None,
        SwarmRegular
    }

    public partial class SwarmCoreRegularThread : FSMBase<ESwarmCoreRegularThread, int>
    {
        public async override Task Init()
        {
            SwarmCoreRegularTask.Set(ES1.Action, ESwarmCoreRegular.AccessToken, 0);

            Set(ES1.Action, ESwarmCoreRegularThread.SwarmRegular, 0);
        }

        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case ESwarmCoreRegularThread.None:
                    Set(ES1.Finish, ESwarmCoreRegularThread.None, 0);
                    break;

                case ESwarmCoreRegularThread.SwarmRegular:
                    switch(S3)
                    {
                        case 0:
                            await SwarmCoreRegularTask.Run();

                            if(SwarmCoreRegularTask.key == EHandshakeKey.Finish)
                            {
                                if(SwarmCoreRegularTask.isError)
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
