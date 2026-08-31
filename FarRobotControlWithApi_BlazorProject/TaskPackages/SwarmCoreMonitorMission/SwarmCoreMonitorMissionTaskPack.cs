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
        public async Task<bool> GetRunningMissionList()
        {
            if(await IDataLib.GetRunningMissionTableList())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsGetRunningMissionList()
        {
            if(IDataLib.ListRunningMission.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsNextRunningMissionTarget()
        {
            if(IDataLib.StartedIndex < IDataLib.ListRunningMission.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> GetRunningMissionTarget()
        {
            if (await IDataLib.GetRunningMissionTableTarget())
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
            foreach(FlowBase flow in IDataLib.TargetRunningMission.Flows)
            {
                if(!await _getProgressByFlowId(flow))
                {
                    return false;
                }
            }

            var activeFlows = IDataLib.TargetRunningMission.Flows.Where(x => !x.IsCancel).ToList();

            if (activeFlows.Count > 0 && activeFlows.All(x => x.IsFinish))
            {
                IDataLib.TargetRunningMission.FinishTime = activeFlows.Max(x => x.FinishTime);
                IDataLib.TargetRunningMission.MissionState = EMissionState.COMPLETED.ToString();
            }
            else if(activeFlows.Any(x => x.IsError))
            {
                IDataLib.TargetRunningMission.MissionState = EMissionState.FAILED.ToString();
            }

            return true;
        }

        async Task<bool> _getProgressByFlowId(FlowBase flow)
        {
            if (string.IsNullOrEmpty(flow.FlowId) || flow.IsFinish || flow.IsCancel || flow.IsError)
                return true;

            IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.flowId = flow.FlowId;

            if (await IAmrControlOp.GetProgressByFlowId(amrControl))
            {
                flow.State = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.state;
                flow.StateString = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.state_string;
                flow.CompletePercent = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.complete_percent;
                flow.TaskId = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.task_ids.FirstOrDefault();

                if(!await _getProgressByTaskId(flow))
                {
                    return false;
                }

                if (string.Equals(flow.StateString, "COMPLETED", StringComparison.OrdinalIgnoreCase))
                {
                    string updateTime = IAmrControlPack.Packages[amrControl].property.farRobot.flowProgress.response.data.updated_timestring;
                    flow.FinishTime = DateTimeOffset.Parse(updateTime).DateTime;
                }

                return true;
            }
            else
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;

                if(_isFlowNotFound(nlog))
                {
                    await IDataLib.WriteNLogError(nlog);
                    return true;
                }

                await IDataLib.WriteNLogError(nlog);
                return false;
            }
        }

        bool _isFlowNotFound(string errorLog)
        {
            return errorLog.Contains("HTTP 404", StringComparison.OrdinalIgnoreCase)
                && errorLog.Contains("flow not found", StringComparison.OrdinalIgnoreCase);
        }

        async Task<bool> _getProgressByTaskId(FlowBase flow)
        {
            if (string.IsNullOrEmpty(flow.TaskId))
                return true;

            IAmrControlPack.Packages[amrControl].property.farRobot.taskProgress.taskId = flow.TaskId;

            if(await IAmrControlOp.GetProgressByTaskId(amrControl))
            {
                flow.StatusCode = IAmrControlPack.Packages[amrControl].property.farRobot.taskProgress.response.data.status_code;
                flow.StatusMessage = IAmrControlPack.Packages[amrControl].property.farRobot.taskProgress.response.data.status_msg;

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
            foreach(FlowBase flow in IDataLib.TargetRunningMission.Flows)
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
            foreach(FlowBase flow in IDataLib.TargetRunningMission.Flows)
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
