using CommonLibraryB.Tools.LogWritter;
using FarRobotControlWithApi_BlazorProject.DTOModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data.Interface;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.Data
{
    public partial class SwarmCoreRegularDataLibary
    {

        readonly INLogWritterObservable INLogWritter;

        public SwarmCoreRegularDataLibary(INLogWritterObservable INLogWritter)
        {
            this.INLogWritter = INLogWritter;
        }

        async Task _writeNLogError(string log)
        {
            await INLogWritter.NotifyNLog(EStatus.Error, log);
        }

        async Task _writeNLogInform(string log)
        {
            await INLogWritter.NotifyNLog(EStatus.Info, log);
        }
    }

    public partial class SwarmCoreRegularDataLibary : ISwarmCoreRegularDataLibrary
    {
        AccessTokenDto _accessToken { get; set; } = new AccessTokenDto();

        public AccessTokenDto TokenInform
        {
            get
            {
                return _accessToken;
            }
            set
            {
                _accessToken = value;
            }
        }

        public async Task WriteNLogError(string log)
        {
            await _writeNLogError(log);
        }

        public async Task WriteNLogInform(string log)
        {
            await _writeNLogInform(log);
        }
    }
}
