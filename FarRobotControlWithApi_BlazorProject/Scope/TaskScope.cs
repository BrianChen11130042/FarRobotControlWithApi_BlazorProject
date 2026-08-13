using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreRegular;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreRegular;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreSetMission;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {

        public SwarmCoreSetMissionTaskPack<EAmrControl> swarmCoreSetMissionTaskPack;

        public SwarmCoreSetMissionTask swarmCoreSetMissionTask;

        void InitSetMissionTask()
        {
            swarmCoreSetMissionTaskPack = new SwarmCoreSetMissionTaskPack<EAmrControl>(EAmrControl.AmrControl,
                                                                                       amrControlLibrary,
                                                                                       amrControlLibrary,
                                                                                       swarmCoreSetMissionDataLibrary);

            swarmCoreSetMissionTask = new SwarmCoreSetMissionTask(swarmCoreSetMissionTaskPack);
            swarmCoreSetMissionTask.Set(ES1.None, ESetMission.None, 0);
        }

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
