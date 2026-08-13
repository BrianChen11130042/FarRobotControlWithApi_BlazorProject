using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreRegular;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SystemControl
{
    public partial class SystemControlThread
    {
        readonly InitialTask initialTask;
        readonly ErrorTask errorTask;

        public SystemControlThread(InitialTask initialTask, ErrorTask errorTask)
        {
            this.initialTask = initialTask;
            this.errorTask = errorTask;

            interval = 1;
        }
    }

    public enum ESystemControlThread
    {
        None,
        InitialTask,
        ErrorTask,
    }

    public partial class SystemControlThread : FSMBase<ESystemControlThread, int>
    {
        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case ESystemControlThread.None:
                    Set(ES1.Finish, ESystemControlThread.None, 0);
                    break;

                case ESystemControlThread.InitialTask:
                    switch(S3)
                    {
                        case 0:
                            initialTask.Set(ES1.Action, EInitialTask.StartInit, 0);
                            Set(10);
                            break;

                        case 10:
                            await initialTask.Run();

                            if(initialTask.key == EHandshakeKey.Finish)
                            {
                                if(initialTask.isError)
                                {
                                    Set(ES1.Error, ESystemControlThread.None, 0);
                                }
                                else
                                {
                                    Set(ES1.Finish, ESystemControlThread.None, 0);
                                }
                            }
                            else
                            {
                                Set(10);
                            }
                            break;
                    }
                    break;

                case ESystemControlThread.ErrorTask:
                    switch(S3)
                    {
                        case 0:
                            errorTask.Set(ES1.Action, EErrorTask.StartError, 0);
                            Set(10);
                            break;

                        case 10:
                            await errorTask.Run();

                            if(errorTask.key == EHandshakeKey.Finish)
                            {
                                Set(ES1.Finish, ESystemControlThread.None, 0);
                            }
                            else
                            {
                                Set(10);
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
            Set(ES1.Idle, ESystemControlThread.None, 0);
        }

        public async override Task Finish()
        {
            isError = false;
            key = EHandshakeKey.Finish;
            Set(ES1.Idle, ESystemControlThread.None, 0);
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
