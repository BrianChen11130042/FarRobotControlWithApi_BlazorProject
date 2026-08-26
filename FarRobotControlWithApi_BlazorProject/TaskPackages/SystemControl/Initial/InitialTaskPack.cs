using CommonLibraryB.Library.AmrControl.Adapter;
using CommonLibraryB.Library.AmrControl.Package;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Initial.Interface;

namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Initial
{
    public partial class InitialTaskPack<EAmrControl>
    {
        readonly EAmrControl amrControl;

        readonly IAmrControlPackage<EAmrControl> IAmrControlPack;
        readonly IAmrControlAdapter<EAmrControl> IAmrControlOp;

        readonly IInitialDataLibrary IDataLib;

        public InitialTaskPack(EAmrControl amrControl, 
                               IAmrControlPackage<EAmrControl> IAmrControlPack,
                               IAmrControlAdapter<EAmrControl> IAmrControlOp,
                               IInitialDataLibrary IDataLib)
        {
            this.amrControl = amrControl;
            this.IAmrControlPack = IAmrControlPack;
            this.IAmrControlOp = IAmrControlOp;
            this.IDataLib = IDataLib;
        }

        const string info = "Inform";

        const string err = "Error";
    }

    public partial class InitialTaskPack<EAmrControl> : IInitialTaskPack
    {
        public async Task<bool> InitAmrMissionInQueue()
        {
            if(await IDataLib.InitAmrMissionInQueue())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> InitSwarmCore()
        {
            if (!await IAmrControlOp.GetAccessToken(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }
            
            if(!await IAmrControlOp.GetFlowName(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }

            IDataLib.ListFlowName = IAmrControlPack.Packages[amrControl].property.farRobot
                                                   .flowName.response.swarm_data.SelectMany(x => x.flows)
                                                                                .Where(x => !string.IsNullOrEmpty(x))
                                                                                .Distinct()
                                                                                .ToList();

            if(!await IAmrControlOp.GetScanAmr(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }

            IDataLib.ListAmrSerialNumber = IAmrControlPack.Packages[amrControl].property.farRobot
                                                          .scanAmr.response.robots.Select(x => x.robot_id)
                                                                                  .Where(x => !string.IsNullOrEmpty(x))
                                                                                  .Distinct()
                                                                                  .ToList();

            if(!await IAmrControlOp.GetCellStatus(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }

            IDataLib.ListCellName = IAmrControlPack.Packages[amrControl].property.farRobot
                                                   .cellStatus.response.cells.Select(x => x.display_name)
                                                                             .Where(x => !string.IsNullOrEmpty(x))
                                                                             .Distinct()
                                                                             .ToList();

            return true;
        }

        public async Task NotifyMissionUpdated()
        {
            await IDataLib.NotifyMissionUpdated();
        }

        public async Task NotifyMissionParamUpdated()
        {
            await IDataLib.NotifyMissionParamUpdated();
        }

        public async Task NotifyInitialSuccess()
        {
            await IDataLib.NotifyIntialResult(true, "Init Success");
        }

        public async Task NotifyInitialFail()
        {
            await IDataLib.NotifyIntialResult(false, "Init Fail");
        }
    }
}
