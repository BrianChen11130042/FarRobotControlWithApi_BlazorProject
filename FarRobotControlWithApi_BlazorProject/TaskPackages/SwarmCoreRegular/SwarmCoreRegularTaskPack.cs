using CommonLibraryB.Library.AmrControl;
using CommonLibraryB.Library.AmrControl.Adapter;
using CommonLibraryB.Library.AmrControl.Package;
using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreRegular.Interface;
using Microsoft.AspNetCore.Http;

namespace FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreRegular
{
    public partial class SwarmCoreRegularTaskPack<EAmrControl>
    {
        readonly EAmrControl amrControl;

        readonly IAmrControlPackage<EAmrControl> IAmrControlPack;
        readonly IAmrControlAdapter<EAmrControl> IAmrControlOp;

        readonly ISwarmCoreRegularDataLibrary IDataLib;


        public SwarmCoreRegularTaskPack(EAmrControl amrControl, 
                                        IAmrControlPackage<EAmrControl> IAmrControlPack,
                                        IAmrControlAdapter<EAmrControl> IAmrControlOp,
                                        ISwarmCoreRegularDataLibrary IDataLib)
        {
            this.amrControl = amrControl;

            this.IAmrControlPack = IAmrControlPack;
            this.IAmrControlOp = IAmrControlOp;

            this.IDataLib = IDataLib;
        }

        const string info = "Inform";

        const string err = "Error";
    }

    public partial class SwarmCoreRegularTaskPack<EAmrControl> : ISwarmCoreRegularTaskPack
    {
        public bool IsGetAccessToken()
        {
            TimeSpan span = DateTime.Now - IDataLib.TokenInform.retrieveTime;

            if (span.TotalDays >= 5 )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> GetAccessToken()
        {
            if(await IAmrControlOp.GetAccessToken(amrControl))
            {
                IDataLib.TokenInform.accessToken = IAmrControlPack.Packages[amrControl].property.farRobot.accessToken.response.access_token;
                IDataLib.TokenInform.tokenType = IAmrControlPack.Packages[amrControl].property.farRobot.accessToken.response.token_type;
                IDataLib.TokenInform.retrieveTime = DateTime.Now;

                return true;
            }
            else
            {
                string nlog = IAmrControlPack.Packages[amrControl].errorLog;
                await IDataLib.WriteNLogError(nlog);
                return false;
            }
        }

    }
}
