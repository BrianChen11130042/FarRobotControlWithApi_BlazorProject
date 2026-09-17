using System.ComponentModel.DataAnnotations;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class MoveArtifactsFlowTable : FlowBase
    {
        [MaxLength(500)]
        public string? CellName { get; set; }


        //Emb
        [MaxLength(500)]
        public string? EmbArtifactId { get; set; }

        public int EmbStartParam { get; set; }

        public int EmbFinishParam { get; set; }

        public int EmbErrorParam { get; set; }

        [MaxLength(500)]
        public string? Emb_LiveInfo_Status { get; set; }

        [MaxLength(500)]
        public string? Emb_LiveInfo_ErrorCode { get; set; }


        //Ext
        [MaxLength(500)]
        public string? ExtArtifactId { get; set; }

        public int ExtStartParam { get; set; }

        public int ExtFinishParam { get; set; }

        public int ExtErrorParam { get; set; }

        [MaxLength(500)]
        public string? Ext_LiveInfo_Status { get; set; }

        [MaxLength(500)]
        public string? Ext_LiveInfo_ErrorCode { get; set; }
    }
}
