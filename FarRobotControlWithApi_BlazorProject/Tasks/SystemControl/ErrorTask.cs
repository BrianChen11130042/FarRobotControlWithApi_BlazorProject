using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Error.Interface;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SystemControl
{
    public partial class ErrorTask : IErrorTaskPack
    {
        readonly IErrorTaskPack pack;

        public ErrorTask(IErrorTaskPack pack)
        {
            this.pack = pack;

            interval = 1;
        }

        public Task NotifyDisconnect()
        {
            return pack.NotifyDisconnect();
        }
    }

    public enum EErrorTask
    {
        None,
        StartError
    }

    public partial class ErrorTask : FSMBase<EErrorTask, int>
    {
        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case EErrorTask.None:
                    Set(ES1.Finish, EErrorTask.None, 0);
                    break;

                case EErrorTask.StartError:
                    switch(S3)
                    {
                        case 0:
                            await NotifyDisconnect();
                            Set(ES1.Finish, EErrorTask.None, 0);
                            break;
                    }
                    break;
            }
        }

        public async override Task Finish()
        {
            isError = false;
            key = EHandshakeKey.Finish;
            Set(ES1.Idle, EErrorTask.None, 0);
        }

        public async override Task Error()
        {
            //throw new NotImplementedException();
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
