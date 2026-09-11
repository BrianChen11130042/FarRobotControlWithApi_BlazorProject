using CommonLibraryB.Library.AmrControl.Adapter;
using CommonLibraryB.Library.AmrControl.Package;
using CommonLibraryB.Library.AmrControl.Property.JsonModel.FarRobotSwarmCoreJson;
using FarRobotControlWithApi_BlazorProject.DTOModel;
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

            IAmrControlPack.Packages[amrControl].property.farRobot.flowName.fleetName =
                string.IsNullOrWhiteSpace(IAmrControlPack.Packages[amrControl].config.fleetName) ? 
                                                     "NODATA" : IAmrControlPack.Packages[amrControl].config.fleetName;

            if (!await IAmrControlOp.GetFlowName(amrControl))
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

            List<ScanAmrInfo> listRobot = IAmrControlPack.Packages[amrControl].property.farRobot
                                                         .scanAmr.response.robots.Where(x => !string.IsNullOrEmpty(x.robot_id))
                                                                                 .ToList();

            IDataLib.DcAmrArtifactMap = listRobot.GroupBy(x => x.robot_id)
                                                 .ToDictionary(a => a.Key, 
                                                               a => _getListArtifact(a.First().artifacts));

            IAmrControlPack.Packages[amrControl].property.farRobot.cellStatus.map_name =
                string.IsNullOrWhiteSpace(IAmrControlPack.Packages[amrControl].config.mapName) ? 
                                                       "NODATA" : IAmrControlPack.Packages[amrControl].config.mapName;

            if (!await IAmrControlOp.GetCellStatus(amrControl))
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }

            IDataLib.ListCellName = IAmrControlPack.Packages[amrControl].property.farRobot
                                                   .cellStatus.response.cells.Where(x => !string.IsNullOrEmpty(x.map)
                                                                                     &&  !string.IsNullOrEmpty(x.area_id)
                                                                                     && !string.IsNullOrEmpty(x.display_name))
                                                                             .Select(x => $"{x.map}@{x.area_id}@{x.display_name}")
                                                                             .Distinct()
                                                                             .ToList();

            return true;
        }

        List<ArtifactInformDto> _getListArtifact(string artifacts)
        {
            List<ArtifactInformDto> result = new List<ArtifactInformDto>();

            if(string.IsNullOrWhiteSpace(artifacts))
                return result;

            string[] parts = artifacts.Split('@', 2);

            if(parts.Length >= 2 && 
               !string.IsNullOrWhiteSpace(parts[0]) &&
               !string.IsNullOrWhiteSpace(parts[1]))
            {
                result.Add(new ArtifactInformDto() 
                {
                    Type = parts[0].Trim(),
                    Id = parts[1].Trim()
                });
            }

            return result;

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
