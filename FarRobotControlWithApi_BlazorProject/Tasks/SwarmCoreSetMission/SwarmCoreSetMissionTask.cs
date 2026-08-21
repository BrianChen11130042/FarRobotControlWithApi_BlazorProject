using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission.Interface;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreSetMission
{
    public partial class SwarmCoreSetMissionTask : ISwarmCoreSetMissionTaskPack
    {
        readonly ISwarmCoreSetMissionTaskPack pack;

        public SwarmCoreSetMissionTask(ISwarmCoreSetMissionTaskPack pack)
        {
            this.pack = pack;
            interval = 1;
        }

        public Task<bool> GetNextMission()
        {
            return pack.GetNextMission();
        }

        public bool IsNewMission()
        {
            return pack.IsNewMission();
        }

        public bool IsCancelMission()
        {
            return pack.IsCancelMission();
        }

        public Task<bool> DispatchNewMission()
        {
            return pack.DispatchNewMission();
        }

        public Task<bool> UpsertMissionTable()
        {
            return pack.UpsertMissionTable();
        }

        public Task NotifyMissionUpdated()
        {
            return pack.NotifyMissionUpdated();
        }

        public Task<bool> DispatchCancelMission()
        {
            return pack.DispatchCancelMission();
        }
    }

    public enum ESetMission
    {
        None,
        CheckMission,

        NewMission,
        CancelMission
    }

    public partial class SwarmCoreSetMissionTask : FSMBase<ESetMission, int>
    {
        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case ESetMission.None:
                    Set(ES1.Finish, ESetMission.None, 0);
                    break;

                case ESetMission.CheckMission:
                    switch(S3)
                    {
                        case 0:
                            if(await GetNextMission())
                            {
                                if(IsNewMission())
                                {
                                    Set(ESetMission.NewMission, 0);
                                }
                                else if(IsCancelMission())
                                {
                                    Set(ESetMission.CancelMission, 0);
                                }
                                else
                                {
                                    Set(0);
                                }
                            }
                            else
                            {
                                SaveState();
                                Set(ES1.Error, ESetMission.None, 0);
                            }
                            break;
                    }
                    break;

                case ESetMission.NewMission:
                    switch(S3)
                    {
                        case 0:
                            if(await DispatchNewMission())
                            {
                                Set(10);
                            }
                            else
                            {
                                SaveState();
                                Set(ES1.Error, ESetMission.None, 0);
                            }
                            break;

                        case 10:
                            if(await UpsertMissionTable())
                            {
                                await NotifyMissionUpdated();
                                Set(ESetMission.CheckMission, 0);

                            }
                            else
                            {
                                SaveState();
                                Set(ES1.Error, ESetMission.None, 0);
                            }
                            break;
                    }
                    break;

                case ESetMission.CancelMission:
                    switch(S3)
                    {
                        case 0:
                            if(await DispatchCancelMission())
                            {
                                Set(10);
                            }
                            else
                            {
                                SaveState();
                                Set(ES1.Error, ESetMission.None, 0);
                            }
                            break;

                        case 10:
                            if (await UpsertMissionTable())
                            {
                                await NotifyMissionUpdated();
                                Set(ESetMission.CheckMission, 0);

                            }
                            else
                            {
                                SaveState();
                                Set(ES1.Error, ESetMission.None, 0);
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
            Set(ES1.Idle, ESetMission.None, 0);
        }

        public async override Task Finish()
        {
            isError = false;
            key = EHandshakeKey.Finish;
            Set(ES1.Idle, ESetMission.None, 0);
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
