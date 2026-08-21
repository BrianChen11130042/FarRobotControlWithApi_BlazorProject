using FarRobotControlWithApi_BlazorProject.EFModel;
using FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable.Interface;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace FarRobotControlWithApi_BlazorProject.ProjectLibrary.DbTable
{
    public partial class MissionTableLibrary
    {
        readonly IServiceProvider serviceProvider;

        public MissionTableLibrary(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        List<AmrMissionTable> _listAmrMissionInQueue { get; set; } = new List<AmrMissionTable>();

        public List<AmrMissionTable> listAmrMissionInQueue
        {
            get
            {
                return _listAmrMissionInQueue;
            }
            set
            {
                _listAmrMissionInQueue = value;
            }
        }
    }

    public partial class MissionTableLibrary : IMissionTableOperate
    {
        public async Task<(bool status, string msg)> InitAmrMissionInQueue()
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    SwarmCoreDbContext context = scope.ServiceProvider.GetRequiredService<SwarmCoreDbContext>();

                    List<AmrMissionTable> list = await context.AmrMissionTables.Include(x => x.Flows)
                                                                               .AsNoTracking()
                                                                               .Where(x => x.FinishTime == null
                                                                                        && x.FlowCount != 0)
                                                                               .OrderBy(x => x.EstablishTime)
                                                                               .ToListAsync();

                    listAmrMissionInQueue.Clear();
                    listAmrMissionInQueue = list;

                    return (true, string.Empty);

                }
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<AmrMissionTable> GetNextMissionTable()
        {
            AmrMissionTable cancelMission = listAmrMissionInQueue.Where(x => x.IsFinish == false
                                                                          && x.CancelRequest == true
                                                                          && x.IsCancel == false
                                                                          && x.FlowCount != 0
                                                                          && x.Flows.Count == x.FlowCount)
                                                                 .OrderBy(x => x.EstablishTime)
                                                                 .FirstOrDefault();

            if(cancelMission != null)
            {
                return cancelMission;
            }

            AmrMissionTable newMission = listAmrMissionInQueue.Where(x => x.IsStart == false
                                                                       && x.IsFinish == false
                                                                       && x.CancelRequest == false
                                                                       && x.IsCancel == false
                                                                       && x.FlowCount != 0
                                                                       && x.Flows.Count == x.FlowCount)
                                                              .OrderBy(x => x.EstablishTime)
                                                              .FirstOrDefault();

            return newMission;
        }

        public async Task<List<AmrMissionTable>> GetStartedMissionTableList()
        {
            List<AmrMissionTable> list = listAmrMissionInQueue.Where(x => x.IsStart == true
                                                                       && x.IsFinish == false
                                                                       && x.IsCancel == false
                                                                       && x.FlowCount != 0
                                                                       && x.Flows.Count == x.FlowCount
                                                                       && x.StartTime.HasValue
                                                                       && x.StartTime.Value <= DateTime.Now.AddSeconds(-3))
                                                              .OrderBy(x => x.EstablishTime)
                                                              .ToList();

            return list;
        }

        public async Task<(bool status, string msg)> UpsertMissionTable(AmrMissionTable data)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    SwarmCoreDbContext context = scope.ServiceProvider.GetRequiredService<SwarmCoreDbContext>();

                    var target = await context.AmrMissionTables.FirstOrDefaultAsync(x => x.Id == data.Id);

                    if(target != null)
                    {
                        context.Entry(target).CurrentValues.SetValues(data);
                    }
                    else
                    {
                        context.AmrMissionTables.Add(data);
                    }

                    await context.SaveChangesAsync();

                    _upsertMissionInQueue(data);

                    return (true, string.Empty);
                }
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }
        }

        void _upsertMissionInQueue(AmrMissionTable data)
        {
            var target = listAmrMissionInQueue.FirstOrDefault(x => x.Id == data.Id);

            if(target != null)
            {
                target.AmrSerialNumber = data.AmrSerialNumber;
                target.Priority = data.Priority;
                target.FlowCount = data.FlowCount;
                target.EstablishTime = data.EstablishTime;
                target.StartTime = data.StartTime;
                target.ErrorCode = data.ErrorCode;
                target.ErrorMessage = data.ErrorMessage;
                target.FinishTime = data.FinishTime;
                target.CancelRequest = data.CancelRequest;
                target.CancelTime = data.CancelTime;
            }
            else
            {
                listAmrMissionInQueue.Add(data);
            }
        }

        public async Task<(bool status, string msg)> UpsertFlow<T>(T data) where T : FlowBase
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    SwarmCoreDbContext context = scope.ServiceProvider.GetRequiredService<SwarmCoreDbContext>();

                    var target = await context.Set<T>().FirstOrDefaultAsync(x => x.Id == data.Id);

                    if(target != null)
                    {
                        context.Entry<T>(target).CurrentValues.SetValues(data);
                    }
                    else
                    {
                        context.Set<T>().Add(data);
                    }

                    await context.SaveChangesAsync();

                    _upsertFlowInQueue<T>(data);

                    return (true, string.Empty);
                }
            }
            catch(Exception ex)
            {
                return (false, ex.Message);
            }
        }

        void _upsertFlowInQueue<T>(T data) where T : FlowBase
        {
            var amrMission = listAmrMissionInQueue.FirstOrDefault(x => x.Id == data.MissionId);

            if (amrMission == null)
                return;

            var target = amrMission.Flows.FirstOrDefault(x => x.Id == data.Id);

            if (target != null)
            {
                target.MissionId = data.MissionId;
                target.AmrSerialNumber = data.AmrSerialNumber;
                target.FlowId = data.FlowId;
                target.Priority = data.Priority;
                target.State = data.State;
                target.StateString = data.StateString;
                target.CompletePercent = data.CompletePercent;
                target.EstablishTime = data.EstablishTime;
                target.StartTime = data.StartTime;
                target.ErrorCode = data.ErrorCode;
                target.ErrorMessage = data.ErrorMessage;
                target.FinishTime = data.FinishTime;
                target.CancelRequest = data.CancelRequest;
                target.CancelTime = data.CancelTime;

                switch(data)
                {
                    case MoveFlowTable moveData when target is MoveFlowTable moveTarget:
                        moveTarget.CellName = moveData.CellName;
                        break;

                    case ChargeFlowTable chargeData when target is ChargeFlowTable chargeTarget:
                        chargeTarget.CellName = chargeData.CellName;
                        chargeTarget.Percentage = chargeData.Percentage;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                amrMission.Flows.Add(data);
            }
        }
    }
}
