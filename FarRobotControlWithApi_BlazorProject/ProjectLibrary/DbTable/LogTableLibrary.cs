using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using Microsoft.EntityFrameworkCore;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable
{

    public partial class LogTableLibrary
    {
        readonly IServiceProvider serviceProvider;

        public LogTableLibrary(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        List<LogTable> _listLogData { get; set; } = new List<LogTable>();

        public List<LogTable> listLogData
        {
            get
            {
                return _listLogData;
            }
            set
            {
                _listLogData = value;
            }
        }
    }

    public partial class LogTableLibrary : ILogTableOperate
    {
        public async Task<(bool status, string msg)> AddLogData(LogTable data)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    SwarmCoreDbContext context = scope.ServiceProvider.GetRequiredService<SwarmCoreDbContext>();

                    context.LogTables.Add(data);

                    await context.SaveChangesAsync();

                    UpsertLogData(data);

                    return (true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        void UpsertLogData(LogTable data)
        {

            listLogData.Add(data);

            DateTime timePoint = DateTime.Now.AddDays(-5);

            listLogData = listLogData.Where(x => x.RecordTime != null 
                                              && x.RecordTime >= timePoint)
                                     .OrderByDescending(x => x.RecordTime)
                                     .Take(100)
                                     .ToList();
        }
    }
}
