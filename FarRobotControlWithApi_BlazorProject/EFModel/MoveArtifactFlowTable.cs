using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class MoveArtifactFlowTable : FlowBase
    {
        [MaxLength(500)]
        public string? CellName { get; set; }

        [MaxLength(500)]
        public string? EmbArtifactId { get; set; }

        public int StartParam { get; set; }

        public int FinishParam { get; set; }

        public int ErrorParam { get; set; }

        [MaxLength(500)]
        public string? LiveInfo_Status { get; set; }

        [MaxLength(500)]
        public string? LiveInfo_ErrorCode { get; set; }

        [NotMapped]
        public bool EmbWasRunning { get; set; }
    }
}
