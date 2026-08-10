using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreRegular;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreRegular;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        public SwarmCoreRegularTaskPack<EAmrControl> swarmCoreRegularTaskPack;

        public SwarmCoreRegularTask swarmCoreRegularTask;


        void InitRegularTask()
        {
            swarmCoreRegularTaskPack = new SwarmCoreRegularTaskPack<EAmrControl>(EAmrControl.AmrControl,
                                                                                 amrControlLibrary,
                                                                                 amrControlLibrary,
                                                                                 swarmCoreRegularDataLibary);

            swarmCoreRegularTask = new SwarmCoreRegularTask(swarmCoreRegularTaskPack);
            swarmCoreRegularTask.Set(ES1.None, ESwarmCoreRegular.None, 0);
        }
    }
}
