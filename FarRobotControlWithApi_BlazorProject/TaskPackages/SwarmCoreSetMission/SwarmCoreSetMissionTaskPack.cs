using CommonLibraryB.Library.AmrControl.Adapter;
using CommonLibraryB.Library.AmrControl.Package;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils.Design;
using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission.Interface;

namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission
{

    public partial class SwarmCoreSetMissionTaskPack<EAmrControl>
    {
        readonly EAmrControl amrControl;

        readonly IAmrControlPackage<EAmrControl> IAmrControlPack;
        readonly IAmrControlAdapter<EAmrControl> IAmrControlOp;

        readonly ISwarmCoreSetMissionDataLibrary IDataLib;

        public SwarmCoreSetMissionTaskPack(EAmrControl amrControl,
                                           IAmrControlPackage<EAmrControl> IAmrControlPack,
                                           IAmrControlAdapter<EAmrControl> IAmrControlOp,
                                           ISwarmCoreSetMissionDataLibrary IDataLib)
        {
            this.amrControl = amrControl;

            this.IAmrControlPack = IAmrControlPack;
            this.IAmrControlOp = IAmrControlOp;

            this.IDataLib = IDataLib;
        }

        const string info = "Inform";

        const string err = "Error";
    }

    public partial class SwarmCoreSetMissionTaskPack<EAmrControl> : ISwarmCoreSetMissionTaskPack
    {
        public async Task<bool> GetNextMission()
        {
            if (await IDataLib.GetNextMissionTable())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsNewMission()
        {
            if (string.Equals(IDataLib.AmrMission.MissionState,
                             EMissionState.DISPATCH_REQUEST.ToString(),
                             StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCancelMission()
        {
            if (string.Equals(IDataLib.AmrMission.MissionState,
                              EMissionState.CANCEL_REQUEST.ToString(),
                              StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsRetryMission()
        {
            if (string.Equals(IDataLib.AmrMission.MissionState,
                              EMissionState.RETRY_REQUEST.ToString(),
                              StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DispatchNewMission()
        {
            foreach (FlowBase flow in IDataLib.AmrMission.Flows)
            {
                switch (flow)
                {
                    case MoveFlowTable moveFlow:
                        if (!await _dispatchMoveFlow(moveFlow))
                        {
                            return false;
                        }
                        break;

                    case ChargeFlowTable chargeFlow:
                        if (!await _dispatchChargeFlow(chargeFlow))
                        {
                            return false;
                        }
                        break;

                    default:
                        break;
                }
            }

            if (IDataLib.AmrMission.Flows.All(x => x.IsStart))
            {
                IDataLib.AmrMission.MissionState = EMissionState.RUNNING.ToString();
                IDataLib.AmrMission.StartTime = DateTime.Now;
            }

            return true;
        }

        async Task<bool> _dispatchMoveFlow(MoveFlowTable moveFlow)
        {
            if (moveFlow.IsStart == true)
                return true;

            IAmrControlPack.Packages[amrControl].property.farRobot.moveFlow.post.args.priority = moveFlow.Priority.ToString();
            IAmrControlPack.Packages[amrControl].property.farRobot.moveFlow.post.args.Params.Node4.assigned_robot = moveFlow.AmrSerialNumber;
            IAmrControlPack.Packages[amrControl].property.farRobot.moveFlow.post.args.Params.Node4.goal_tynXx = moveFlow.CellName;

            if (await IAmrControlOp.SetMoveFlow(amrControl))
            {
                moveFlow.FlowId = IAmrControlPack.Packages[amrControl].property.farRobot.moveFlow.response.swarm_data.flow_id;
                moveFlow.StartTime = DateTime.Now;
                return true;
            }
            else
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }
        }

        async Task<bool> _dispatchChargeFlow(ChargeFlowTable chargeFlow)
        {
            if (chargeFlow.IsStart == true)
                return true;

            IAmrControlPack.Packages[amrControl].property.farRobot.chargeFlow.post.args.priority = chargeFlow.Priority.ToString();
            IAmrControlPack.Packages[amrControl].property.farRobot.chargeFlow.post.args.Params.Node4.assigned_robot = chargeFlow.AmrSerialNumber;
            IAmrControlPack.Packages[amrControl].property.farRobot.chargeFlow.post.args.Params.Node4.goal_nUvaT = chargeFlow.CellName;
            IAmrControlPack.Packages[amrControl].property.farRobot.chargeFlow.post.args.Params.Node4.percentage_nUvaT = chargeFlow.Percentage.ToString();

            if (await IAmrControlOp.SetChargeFlow(amrControl))
            {
                chargeFlow.FlowId = IAmrControlPack.Packages[amrControl].property.farRobot.chargeFlow.response.swarm_data.flow_id;
                chargeFlow.StartTime = DateTime.Now;
                return true;
            }
            else
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }
        }

        public async Task<bool> DispatchCancelMission()
        {
            foreach (FlowBase flow in IDataLib.AmrMission.Flows)
            {
                if (!await _setCancelByFlowId(flow))
                {
                    return false;
                }
            }

            if (IDataLib.AmrMission.Flows.All(x => x.IsFinish || x.IsCancel))
            {
                DateTime? maxTime = IDataLib.AmrMission.Flows.SelectMany(x => new DateTime?[] { x.FinishTime, x.CancelTime })
                                                             .Where(t => t.HasValue)
                                                             .Max();
                IDataLib.AmrMission.CancelTime = maxTime;
                IDataLib.AmrMission.MissionState = EMissionState.CANCELED.ToString();
            }

            return true;
        }

        async Task<bool> _setCancelByFlowId(FlowBase flow)
        {
            if (!flow.IsStart && string.IsNullOrEmpty(flow.FlowId))
            {
                flow.CancelTime = DateTime.Now;
                return true;
            }

            if (string.IsNullOrEmpty(flow.FlowId) || flow.IsFinish || flow.IsCancel)
                return true;

            IAmrControlPack.Packages[amrControl].property.farRobot.deleteFlow.flowId = flow.FlowId;

            if (await IAmrControlOp.SetDeleteFlowByFlowId(amrControl))
            {
                if (IAmrControlPack.Packages[amrControl].property.farRobot.deleteFlow.response.system_status_code == 200)
                {
                    flow.CancelTime = DateTime.Now;
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }
        }

        public async Task<bool> DispatchRetryMission()
        {
            if(!await _retryNewMission())
            {
                return false;
            }

            if(! await _cancelFailedMission())
            {
                return false;
            }

            if(IDataLib.AmrMission.Flows.Where(f => f.IsError).All(f => f.IsCancel) &&
               IDataLib.AmrMission.Flows.Where(f => !f.IsError && !f.IsCancel).All(f => f.IsStart))
            {
                IDataLib.AmrMission.MissionState = EMissionState.RUNNING.ToString();
            }

            return true;
        }

        async Task<bool> _retryNewMission()
        {
            foreach (FlowBase flow in IDataLib.AmrMission.Flows.Where(f => !f.IsStart && !f.IsFinish && !f.IsCancel && !f.IsError))
            {
                switch (flow)
                {
                    case MoveFlowTable moveFlow:
                        if (!await _dispatchMoveFlow(moveFlow))
                        {
                            return false;
                        }
                        break;

                    case ChargeFlowTable chargeFlow:
                        if (!await _dispatchChargeFlow(chargeFlow))
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

        async Task<bool> _cancelFailedMission()
        {
            foreach (FlowBase flow in IDataLib.AmrMission.Flows.Where(f => f.IsStart && f.IsError && !f.IsFinish && !f.IsCancel))
            {
                if (!await _setCancelByFlowId(flow))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> UpsertMissionTable()
        {
            if (await IDataLib.UpsertMissionTable())
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
