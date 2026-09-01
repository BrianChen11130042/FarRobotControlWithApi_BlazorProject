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

        public bool IsRetryMission()
        {
            return pack.IsRetryMission();
        }

        public bool IsContinueMission()
        {
            return pack.IsContinueMission();
        }

        public Task<bool> DispatchNewMission()
        {
            return pack.DispatchNewMission();
        }

        public Task<bool> DispatchCancelMission()
        {
            return pack.DispatchCancelMission();
        }

        public Task<bool> DispatchRetryMission()
        {
            return pack.DispatchRetryMission();
        }

        public Task<bool> DispatchContinueMission()
        {
            return pack.DispatchContinueMission();
        }

        public Task<bool> UpsertMissionTable()
        {
            return pack.UpsertMissionTable();
        }

        public Task NotifyMissionUpdated()
        {
            return pack.NotifyMissionUpdated();
        }
    }

    public enum ESetMission
    {
        None,
        CheckMission,

        NewMission,
        CancelMission,
        RetryMission,
        ContinueMission
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
                                else if(IsRetryMission())
                                {
                                    Set(ESetMission.RetryMission, 0);
                                }
                                else if(IsContinueMission())
                                {
                                    Set(ESetMission.ContinueMission, 0);
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

                case ESetMission.RetryMission:
                    switch(S3)
                    {
                        case 0:
                            if(await DispatchRetryMission())
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

                case ESetMission.ContinueMission:
                    switch(S3)
                    {
                        case 0:
                            if(await DispatchContinueMission())
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
