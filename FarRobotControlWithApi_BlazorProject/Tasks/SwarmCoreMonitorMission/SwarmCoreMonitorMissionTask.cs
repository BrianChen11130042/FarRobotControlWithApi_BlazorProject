using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreMonitorMission.Interface;

namespace FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreMonitorMission
{
    public partial class SwarmCoreMonitorMissionTask : ISwarmCoreMonitorMissionTaskPack
    {

        readonly ISwarmCoreMonitorMissionTaskPack pack;

        public SwarmCoreMonitorMissionTask(ISwarmCoreMonitorMissionTaskPack pack)
        {
            this.pack = pack;
            interval = 1;
        }

        public Task<bool> GetStartedMissionList()
        {
            return pack.GetStartedMissionList();
        }

        public bool IsGetStartedMissionList()
        {
            return pack.IsGetStartedMissionList();
        }

        public bool IsNextStartedMissionTarget()
        {
            return pack.IsNextStartedMissionTarget();
        }

        public Task<bool> GetStartMissionTarget()
        {
            return pack.GetStartMissionTarget();
        }

        public Task<bool> GetProgressByFlowId()
        {
            return pack.GetProgressByFlowId();
        }
    }

    public enum EMonitorMission
    {
        None,
        CheckStartedMission,

        GetFlowProgress,
        GetArtifactStatus
    }

    public partial class SwarmCoreMonitorMissionTask : FSMBase<EMonitorMission, int>
    {
        public async override Task Action()
        {
            key = EHandshakeKey.Run;

            switch(S2)
            {
                case EMonitorMission.None:
                    Set(ES1.Finish, EMonitorMission.None, 0);
                    break;

                case EMonitorMission.CheckStartedMission:
                    switch(S3)
                    {
                        case 0:
                            if(await GetStartedMissionList())
                            {
                                if(IsGetStartedMissionList())
                                {
                                    Set(10);
                                }
                                else
                                {
                                    Set(0);
                                }
                            }
                            else
                            {
                                SaveState();
                                Set(ES1.Error, EMonitorMission.None, 0);
                            }
                            break;

                        case 10:
                            if(IsNextStartedMissionTarget())
                            {
                                if(await GetStartMissionTarget())
                                {
                                    Set(EMonitorMission.GetFlowProgress, 0);
                                }
                                else
                                {
                                    SaveState();
                                    Set(ES1.Error, EMonitorMission.None, 0);
                                }
                            }
                            else
                            {
                                Set(0);
                            }
                            break;
                    }
                    break;

                case EMonitorMission.GetFlowProgress:
                    switch(S3)
                    {
                        case 0:
                            if(await GetProgressByFlowId())
                            {
                                
                            }
                            else
                            {

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
            Set(ES1.Idle, EMonitorMission.None, 0);
        }

        public async override Task Finish()
        {
            isError = false;
            key = EHandshakeKey.Finish;
            Set(ES1.Idle, EMonitorMission.None, 0);
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
