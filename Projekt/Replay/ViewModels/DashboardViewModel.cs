

namespace Replay.ViewModels;

public class DashboardViewModel {
    public bool IsAdmin {get; set;}
    public int TasksOpen {get; set;}
    public int TasksInProgress {get; set;}
    public int TasksDone {get; set;}
    public int? OperationsAll {get; set;}
    public int? OperationsActive {get; set;}
    public int? OperationsClosed {get; set;}
}