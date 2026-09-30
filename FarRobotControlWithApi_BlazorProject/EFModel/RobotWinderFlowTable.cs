using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class RobotWinderFlowTable : FlowBase
    {
        [MaxLength(500)]
        public string? CellName { get; set; }

        //Ext Winder Unlock
        [MaxLength(500)]
        public string? WinderUnlockArtifactId { get; set; }

        public int WinderUnlockFinishParam { get; set; }

        public int WinderUnlockErrorParam { get; set; }

        [MaxLength(500)]
        public string? WinderUnlock_LiveInfo_Status { get; set; }

        [MaxLength(500)]
        public string? WinderUnlock_LiveInfo_ErrorCode { get; set; }

        [NotMapped]
        public bool WinderUnlockWasRunning { get; set; }

        //Emb TM Robot
        [MaxLength(500)]
        public string? TmRobotArtifactId { get; set; }

        public int TmRobotStartParam { get; set; }

        public int TmRobotFinishParam { get; set; }

        public int TmRobotErrorParam { get; set; }

        [MaxLength(500)]
        public string? TmRobot_LiveInfo_Status { get; set; }

        [MaxLength(500)]
        public string? TmRobot_LiveInfo_ErrorCode { get; set; }

        [NotMapped]
        public bool TmRobotWasRunning { get; set; }

        //Ext Winder Lock
        [MaxLength(500)]
        public string? WinderLockArtifactId { get; set; }

        public int WinderLockFinishParam { get; set; }

        public int WinderLockErrorParam { get; set; }

        [MaxLength(500)]
        public string? WinderLock_LiveInfo_Status { get; set; }

        [MaxLength(500)]
        public string? WinderLock_LiveInfo_ErrorCode { get; set; }

        [NotMapped]
        public bool WinderLockWasRunning { get; set; }
    }
}
