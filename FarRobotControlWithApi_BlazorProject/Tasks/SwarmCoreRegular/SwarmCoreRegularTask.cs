using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreRegular.Interface;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreRegular
{
    public partial class SwarmCoreRegularTask : ISwarmCoreRegularTaskPack
    {
        readonly ISwarmCoreRegularTaskPack pack;

        public SwarmCoreRegularTask(ISwarmCoreRegularTaskPack pack)
        {
            this.pack = pack;
            interval = 1;
        }

        public bool IsGetAccessToken()
        {
            return pack.IsGetAccessToken();
        }

        public Task<bool> GetAccessToken()
        {
            return pack.GetAccessToken();
        }

    }

    public enum ESwarmCoreRegular
    {
        None,
        AccessToken
    }

    public partial class SwarmCoreRegularTask : FSMBase<ESwarmCoreRegular, int>
    {
        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case ESwarmCoreRegular.None:
                    Set(ES1.Finish, ESwarmCoreRegular.None, 0);
                    break;

                case ESwarmCoreRegular.AccessToken:
                    switch(S3)
                    {
                        case 0:
                            if(IsGetAccessToken())
                            {
                                if(await GetAccessToken())
                                {
                                    Set(ESwarmCoreRegular.AccessToken, 0);
                                }
                                else
                                {
                                    SaveState();
                                    Set(ES1.Error, ESwarmCoreRegular.None, 0);
                                }
                            }
                            else
                            {
                                Set(ESwarmCoreRegular.AccessToken, 0);
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
            Set(ES1.Idle, ESwarmCoreRegular.None, 0);
        }

        public async override Task Finish()
        {
            isError = false;
            key = EHandshakeKey.Finish;
            Set(ES1.Idle, ESwarmCoreRegular.None, 0);
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
