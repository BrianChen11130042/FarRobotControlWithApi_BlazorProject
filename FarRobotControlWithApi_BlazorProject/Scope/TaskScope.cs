using CommonLibraryB.Base.FiniteStateMachine;
using FarRobotControlWithApi_BlazorProject.EquipName.AmrControl;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreRegular;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SwarmCoreSetMission;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Error;
using FarRobotControlWithApi_BlazorProject.TaskPackages.SystemControl.Initial;
using FarRobotControlWithApi_BlazorProject.Tasks.Main;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreRegular;
using FarRobotControlWithApi_BlazorProject.Tasks.SwarmCoreSetMission;
using FarRobotControlWithApi_BlazorProject.Tasks.SystemControl;

namespace FarRobotControlWithApi_BlazorProject.Scope
{
    public partial class MachineScope
    {
        public InitialTaskPack<EAmrControl> initialTaskPack;
        public ErrorTaskPack errorTaskPack;

        public InitialTask initialTask;
        public ErrorTask errorTask;

        void _initSysControlTask()
        {
            initialTaskPack = new InitialTaskPack<EAmrControl>(EAmrControl.AmrControl, 
                                                               amrControlLibrary, 
                                                               amrControlLibrary,
                                                               initialDataLibrary);

            errorTaskPack = new ErrorTaskPack(errorDataLibrary);

            initialTask = new InitialTask(initialTaskPack);
            errorTask = new ErrorTask(errorTaskPack);

            initialTask.Set(ES1.None, EInitialTask.None, 0);
            errorTask.Set(ES1.None, EErrorTask.None, 0);
        }


        public SwarmCoreSetMissionTaskPack<EAmrControl> swarmCoreSetMissionTaskPack;

        public SwarmCoreSetMissionTask swarmCoreSetMissionTask;

        void _initSetMissionTask()
        {
            swarmCoreSetMissionTaskPack = new SwarmCoreSetMissionTaskPack<EAmrControl>(EAmrControl.AmrControl,
                                                                                       amrControlLibrary,
                                                                                       amrControlLibrary,
                                                                                       swarmCoreSetMissionDataLibrary);

            swarmCoreSetMissionTask = new SwarmCoreSetMissionTask(swarmCoreSetMissionTaskPack);
            swarmCoreSetMissionTask.Set(ES1.None, ESetMission.None, 0);
        }

        public SwarmCoreRegularTaskPack<EAmrControl> swarmCoreRegularTaskPack;

        public SwarmCoreRegularTask swarmCoreRegularTask;

        void _initRegularTask()
        {
            swarmCoreRegularTaskPack = new SwarmCoreRegularTaskPack<EAmrControl>(EAmrControl.AmrControl,
                                                                                 amrControlLibrary,
                                                                                 amrControlLibrary,
                                                                                 swarmCoreRegularDataLibary);

            swarmCoreRegularTask = new SwarmCoreRegularTask(swarmCoreRegularTaskPack);
            swarmCoreRegularTask.Set(ES1.None, ESwarmCoreRegular.None, 0);
        }

        public SystemControlThread systemControlThread;
        public SwarmCoreRegularThread swarmCoreRegularThread;
        public SwarmCoreSetMissionThread swarmCoreSetMissionThread;

        public MainThread mainThread;

        void _initThreadTask()
        {
            systemControlThread = new SystemControlThread(initialTask, errorTask);
            systemControlThread.Set(ES1.None, ESystemControlThread.None, 0);

            swarmCoreRegularThread = new SwarmCoreRegularThread(swarmCoreRegularTask);
            swarmCoreRegularThread.Set(ES1.None, ESwarmCoreRegularThread.None, 0);

            swarmCoreSetMissionThread = new SwarmCoreSetMissionThread(swarmCoreSetMissionTask);
            swarmCoreSetMissionThread.Set(ES1.None, ESetMissionThread.None, 0);

            mainThread = new MainThread(systemControlThread, swarmCoreRegularThread, swarmCoreSetMissionThread);
            mainThread.Set(ES1.Init, EMainThread.None, 0);
        }

        private CancellationTokenSource _ctsMain;
        private CancellationTokenSource _ctsSystemControl;
        private CancellationTokenSource _ctsRegular;
        private CancellationTokenSource _ctsSetMisison;

        private Task _mainTask;
        private Task _systemControlTask;
        private Task _regularTask;
        private Task _setMisisonTask;

        public void _initThread()
        {
            if(_mainTask == null || _mainTask.IsCompleted)
            {
                _ctsMain = new CancellationTokenSource();
                _mainTask = StartLongRunning(async () => await RunMainAsync(_ctsMain.Token));
            }

            if(_systemControlTask == null || _systemControlTask.IsCompleted)
            {
                _ctsSystemControl = new CancellationTokenSource();
                _systemControlTask = StartLongRunning(async () => await RunSystemControlAsync(_ctsSystemControl.Token));
            }

            if(_regularTask == null || _regularTask.IsCompleted)
            {
                _ctsRegular = new CancellationTokenSource();
                _regularTask = StartLongRunning(async () => await RunRegularAsync(_ctsRegular.Token));
            }

            if(_setMisisonTask == null || _setMisisonTask.IsCompleted)
            {
                _ctsSetMisison = new CancellationTokenSource();
                _setMisisonTask = StartLongRunning(async() => await RunSetMissionAsync(_ctsSetMisison.Token));
            }
        }

        private Task StartLongRunning(Func<Task> func)
        {
            // 開啟「專屬 thread」而不是 ThreadPool
            return Task.Factory.StartNew(
                async () => await func(),
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();
        }

        private async Task RunMainAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await mainThread.Run();

                    await Task.Delay(300, token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Console.WriteLine($"Main 例外: {ex}");
                }
            }
        }

        private async Task RunSystemControlAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await systemControlThread.Run();

                    await Task.Delay(100, token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Console.WriteLine($"System Control 例外: {ex}");
                }
            }
        }

        private async Task RunRegularAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await swarmCoreRegularThread.Run();

                    await Task.Delay(500, token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Console.WriteLine($"Regular 例外: {ex}");
                }
            }
        }

        private async Task RunSetMissionAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await swarmCoreSetMissionThread.Run();

                    await Task.Delay(50, token);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Console.WriteLine($"Set Mission 例外: {ex}");
                }
            }
        }

        public void _stopThread()
        {
            _ctsMain?.Cancel();
            _ctsSystemControl?.Cancel();
            _ctsRegular?.Cancel();
            _ctsSetMisison?.Cancel();

            // 強制等待所有 Task 結束，最多等 60 秒，避免死結
            Task.WaitAll(new[] { _mainTask, _systemControlTask, _regularTask, _setMisisonTask }
                        .Where(t => t != null)
                        .ToArray(), TimeSpan.FromSeconds(60));

        }
    }
}
