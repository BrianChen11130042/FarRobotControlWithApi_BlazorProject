using CommonLibraryB.Library.AmrControl.Adapter;
using CommonLibraryB.Library.AmrControl.Package;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreMonitorMission.Interface;
using System.Threading.Tasks;

namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreMonitorMission
{
    public partial class SwarmCoreMonitorMissionTaskPack<EAmrControl>
    {
        readonly EAmrControl amrControl;

        readonly IAmrControlPackage<EAmrControl> IAmrControlPack;
        readonly IAmrControlAdapter<EAmrControl> IAmrControlOp;

        readonly ISwarmCoreMonitorMissionDataLibrary IDataLib;

        public SwarmCoreMonitorMissionTaskPack(EAmrControl amrControl,
                                               IAmrControlPackage<EAmrControl> IAmrControlPack,
                                               IAmrControlAdapter<EAmrControl> IAmrControlOp,
                                               ISwarmCoreMonitorMissionDataLibrary IDataLib)
        {
            this.amrControl = amrControl;

            this.IAmrControlPack = IAmrControlPack;
            this.IAmrControlOp = IAmrControlOp;

            this.IDataLib = IDataLib;
        }

        const string info = "Inform";

        const string err = "Error";
    }

    public partial class SwarmCoreMonitorMissionTaskPack<EAmrControl> : ISwarmCoreMonitorMissionTaskPack
    {
        public async Task<bool> GetStartedMissionList()
        {
            if(await IDataLib.GetStartedMissionTableList())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsGetStartedMissionList()
        {
            if(IDataLib.ListStartedMission.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsNextStartedMissionTarget()
        {
            if(IDataLib.StartedIndex < IDataLib.ListStartedMission.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> GetStartMissionTarget()
        {
            if (await IDataLib.GetStartedMissionTableTarget())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> GetProgressByFlowId()
        {
            foreach(FlowBase flow in IDataLib.TargetStartedMission.Flows)
            {
                if(!await _getProgressByFlowId(flow))
                {
                    return false;
                }
            }

            if(IDataLib.TargetStartedMission.Flows.All(x => x.IsFinish))
            {
                IDataLib.TargetStartedMission.FinishTime = IDataLib.TargetStartedMission.Flows.Max(x => x.FinishTime);
            }

            return true;
        }

        async Task<bool> _getProgressByFlowId(FlowBase flow)
        {
            if (string.IsNullOrEmpty(flow.FlowId))
                return true;

            IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.flowId = flow.FlowId;

            if (await IAmrControlOp.GetProgressByFlowId(amrControl))
            {
                flow.State = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.state;
                flow.StateString = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.state_string;
                flow.CompletePercent = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.complete_percent;
                
                string updateTime = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.updated_timestring;

                if (string.Equals(flow.StateString, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    flow.FinishTime = DateTimeOffset.Parse(updateTime).DateTime;
                }

                return true;
            }
            else
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }
        }

        public bool IsNeedGetArtifactStatus()
        {
            foreach(FlowBase flow in IDataLib.TargetStartedMission.Flows)
            {
                switch(flow)
                {
                    default:
                        break;
                }
            }

            return false;
        }

        public async Task<bool> GetArtifactStatusByArtifactId()
        {
            foreach(FlowBase flow in IDataLib.TargetStartedMission.Flows)
            {
                switch(flow)
                {
                    default:
                        break;
                }
            }

            return true;
        }

        public async Task<bool> UpsertMissionTable()
        {
            if(await IDataLib.UpsertMissionTable())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task NotifyMissionUpdated()
        {
            await IDataLib.NotifyMissionUpdated();
        }
    }
}
