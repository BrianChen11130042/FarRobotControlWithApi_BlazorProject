using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Initial.Interface;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SystemControl
{
    public partial class InitialTask : IInitialTaskPack
    {
        readonly IInitialTaskPack pack;

        public InitialTask(IInitialTaskPack pack)
        {
            this.pack = pack;

            interval = 1;
        }

        public Task<bool> InitSwarmCore()
        {
            return pack.InitSwarmCore();
        }

        public Task<bool> InitAmrMissionInQueue()
        {
            return pack.InitAmrMissionInQueue();
        }

        public Task NotifyInitialFail()
        {
            return pack.NotifyInitialFail();
        }

        public Task NotifyInitialSuccess()
        {
            return pack.NotifyInitialSuccess();
        }

        public Task NotifyMissionUpdated()
        {
            return pack.NotifyMissionUpdated();
        }

        public Task NotifyMissionParamUpdated()
        {
            return pack.NotifyMissionParamUpdated();
        }
    }

    public enum EInitialTask
    {
        None,
        StartInit
    }

    public partial class InitialTask : FSMBase<EInitialTask, int>
    {
        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case EInitialTask.None:
                    Set(ES1.Finish, EInitialTask.None, 0);
                    break;

                case EInitialTask.StartInit:
                    switch(S3)
                    {
                        case 0:
                            if(await InitAmrMissionInQueue())
                            {
                                await NotifyMissionUpdated();
                                Set(10);
                            }
                            else
                            {
                                await NotifyInitialFail();
                                Set(ES1.Error, EInitialTask.None, 0);
                            }
                            break;

                        case 10:
                            if(await InitSwarmCore())
                            {
                                await NotifyMissionParamUpdated();
                                await NotifyInitialSuccess();
                                Set(ES1.Finish, EInitialTask.None, 0);
                            }
                            else
                            {
                                await NotifyInitialFail();
                                Set(ES1.Error, EInitialTask.None, 0);
                            }
                            break;
                    }
                    break;
            }
        }

        public async override Task Error()
        {
            isError = true;
            key = EHandshakeKey.Finish;
            Set(ES1.Idle, EInitialTask.None, 0);
        }

        public async override Task Finish()
        {
            isError = false;
            key = EHandshakeKey.Finish;
            Set(ES1.Idle, EInitialTask.None, 0);
        }

        public async override Task Idle()
        {
            //throw new NotImplementedException();
        }

        public async override Task Init()
        {
            //throw new NotImplementedException();
        }
    }
}
