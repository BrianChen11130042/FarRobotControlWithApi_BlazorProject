using System.ComponentModel.DataAnnotations;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class ChargeFlowTable : FlowBase
    {
        [MaxLength(500)]
        public string? CellName { get; set; }

        public int Percentage { get; set; }
    }
}
