using CommonLibraryB.Library.AmrControl;
using CommonLibraryB.Library.AmrControl.Config;
using CommonLibraryB.Library.AmrControl.Property;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
using Microsoft.Extensions.DependencyInjection;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        public AmrControlConfigManager<EAmrControl> amrControlConfig;
        public AmrControlPropertyManager<EAmrControl> amrControlProperty;
        public AmrControlLibrary<EAmrControl> amrControlLibrary;

        void _createAmrControl()
        {
            amrControlConfig = provider.GetRequiredService<AmrControlConfigManager<EAmrControl>>();
            amrControlProperty = provider.GetRequiredService<AmrControlPropertyManager<EAmrControl>>();
            amrControlLibrary = provider.GetRequiredService<AmrControlLibrary<EAmrControl>>();
        }

        void _initAmrControl()
        {
            amrControlLibrary.InitPackage();
            amrControlLibrary.InitAdapter();
        }
    }
}
