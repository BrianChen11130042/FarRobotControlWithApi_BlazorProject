using CommonLibraryB.Library.AmrControl.Adapter;
using CommonLibraryB.Library.AmrControl.Package;
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

    }
}
