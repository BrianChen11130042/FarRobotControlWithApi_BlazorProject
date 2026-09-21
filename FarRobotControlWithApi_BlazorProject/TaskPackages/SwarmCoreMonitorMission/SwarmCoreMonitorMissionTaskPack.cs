using CommonLibraryB.Library.AmrControl.Adapter;
using CommonLibraryB.Library.AmrControl.Package;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreMonitorMission.Interface;
using System.Text.Json;
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
            if(IDataLib.RunningIndex < IDataLib.ListRunningMission.Count)
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
            else if(activeFlows.Count == 0
                    && IDataLib.TargetRunningMission.Flows.All(x => x.IsCancel))
            {
                IDataLib.TargetRunningMission.FinishTime = IDataLib.TargetRunningMission.Flows.Where(x => x.CancelTime.HasValue)
                                                                                              .Max(x => x.CancelTime);

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
            return IDataLib.TargetRunningMission.Flows.Any(f =>
            f.IsStart
            && !f.IsError
            && !f.IsFinish
            && !f.IsCancel
            && (
                   (f is MoveArtifactFlowTable moveArtifact 
                      && !string.IsNullOrWhiteSpace(moveArtifact.EmbArtifactId))
                || (f is MoveArtifactsFlowTable moveArtifacts 
                      && !string.IsNullOrWhiteSpace(moveArtifacts.EmbArtifactId)
                      && !string.IsNullOrWhiteSpace(moveArtifacts.ExtArtifactId))
               )
            );
        }

        public async Task<bool> GetArtifactStatusByArtifactId()
        {
            foreach(FlowBase flow in IDataLib.TargetRunningMission.Flows)
            {
                switch(flow)
                {
                    case MoveArtifactFlowTable moveArtifact :
                        if(!await _getArtifactByMoveArtifactFlow(moveArtifact))
                        {
                            return false;
                        }
                        break;

                    case MoveArtifactsFlowTable moveArtifacts :
                        if(!await _getArtifactsByMoveArtifactsFlow(moveArtifacts))
                        {
                            return false;
                        }
                        break;

                    default:
                        break;
                }
            }

            return true;
        }

        async Task<bool> _getArtifactByMoveArtifactFlow(MoveArtifactFlowTable moveArtifact)
        {
            if (!moveArtifact.IsStart || moveArtifact.IsFinish || moveArtifact.IsError || moveArtifact.IsCancel 
                || string.IsNullOrWhiteSpace(moveArtifact.EmbArtifactId) 
                || string.Equals(moveArtifact.StateString, "QUEUED", StringComparison.OrdinalIgnoreCase))
                return true;

            IAmrControlPack.Packages[amrControl].property.farRobot.artifactStatusByArtifactId.artifactId = moveArtifact.EmbArtifactId;

            if (!await IAmrControlOp.GetArtifactStatusByArtifactId(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }

            var response = IAmrControlPack.Packages[amrControl].property.farRobot.artifactStatusByArtifactId.response;
            bool isRunning = response.service != null
                             && response.service.Values.Any(s => string.Equals(s?.response?.status, "running",
                                                                               StringComparison.OrdinalIgnoreCase));

            bool readLiveInfo = isRunning || moveArtifact.EmbWasRunning;

            if (readLiveInfo)
            {
                Dictionary<string, JsonElement> liveInfo = IAmrControlPack.Packages[amrControl].property.farRobot
                                                                      .artifactStatusByArtifactId.response.state.live_info;

                if (liveInfo.TryGetValue("status", out var status))
                {
                    moveArtifact.LiveInfo_Status = status.ToString();
                }

                if (liveInfo.TryGetValue("errorcode", out var errorCode))
                {
                    moveArtifact.LiveInfo_ErrorCode = errorCode.ToString();
                }
            }

            moveArtifact.EmbWasRunning = isRunning;

            return true;
        }

        async Task<bool> _getArtifactsByMoveArtifactsFlow(MoveArtifactsFlowTable moveArtifacts)
        {
            if (!moveArtifacts.IsStart || moveArtifacts.IsFinish || moveArtifacts.IsError || moveArtifacts.IsCancel
                || string.IsNullOrWhiteSpace(moveArtifacts.EmbArtifactId) 
                || string.IsNullOrWhiteSpace(moveArtifacts.ExtArtifactId)
                || string.Equals(moveArtifacts.StateString, "QUEUED", StringComparison.OrdinalIgnoreCase))
                return true;

            //emb
            IAmrControlPack.Packages[amrControl].property.farRobot.artifactStatusByArtifactId.artifactId = moveArtifacts.EmbArtifactId;

            if (!await IAmrControlOp.GetArtifactStatusByArtifactId(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }

            var embResponse = IAmrControlPack.Packages[amrControl].property.farRobot.artifactStatusByArtifactId.response;
            bool isEmbRunning = embResponse.service != null
                                && embResponse.service.Values.Any(s => string.Equals(s?.response?.status, "running",
                                                                                     StringComparison.OrdinalIgnoreCase));

            bool readEmbLiveInfo = isEmbRunning || moveArtifacts.EmbWasRunning;

            if (readEmbLiveInfo)
            {
                Dictionary<string, JsonElement> embLiveInfo = IAmrControlPack.Packages[amrControl].property.farRobot
                                                                         .artifactStatusByArtifactId.response.state.live_info;

                if (embLiveInfo.TryGetValue("status", out var embStatus))
                {
                    moveArtifacts.Emb_LiveInfo_Status = embStatus.ToString();
                }

                if (embLiveInfo.TryGetValue("errorcode", out var embErrorCode))
                {
                    moveArtifacts.Emb_LiveInfo_ErrorCode = embErrorCode.ToString();
                }
            }

            moveArtifacts.EmbWasRunning = isEmbRunning;

            //ext
            IAmrControlPack.Packages[amrControl].property.farRobot.artifactStatusByArtifactId.artifactId = moveArtifacts.ExtArtifactId;

            if (!await IAmrControlOp.GetArtifactStatusByArtifactId(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }

            var extResponse = IAmrControlPack.Packages[amrControl].property.farRobot.artifactStatusByArtifactId.response;
            bool isExtRunning = extResponse.service != null
                                && extResponse.service.Values.Any(s => string.Equals(s?.response?.status, "running",
                                                                                     StringComparison.OrdinalIgnoreCase));

            bool readExtLiveInfo = isExtRunning || moveArtifacts.ExtWasRunning;

            if (readExtLiveInfo)
            {
                Dictionary<string, JsonElement> extLiveInfo = IAmrControlPack.Packages[amrControl].property.farRobot
                                                                         .artifactStatusByArtifactId.response.state.live_info;

                if (extLiveInfo.TryGetValue("extstatus", out var extStatus))
                {
                    moveArtifacts.Ext_LiveInfo_Status = extStatus.ToString();
                }

                if (extLiveInfo.TryGetValue("exterrorcode", out var extErrorCode))
                {
                    moveArtifacts.Ext_LiveInfo_ErrorCode = extErrorCode.ToString();
                }
            }

            moveArtifacts.ExtWasRunning = isExtRunning;

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
