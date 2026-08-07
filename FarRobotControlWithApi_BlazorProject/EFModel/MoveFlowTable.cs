using System.ComponentModel.DataAnnotations;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class MoveFlowTable : FlowBase
    {

        [MaxLength(500)]
        public string? CellName { get; set; }
    }
}
