using System.ComponentModel.DataAnnotations;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class LogTable
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string? LogType { get; set; }

        [MaxLength(500)]
        public string? Equipment { get; set; }

        public string? Msg { get; set; }

        public DateTime RecordTime { get; set; }
    }
}
