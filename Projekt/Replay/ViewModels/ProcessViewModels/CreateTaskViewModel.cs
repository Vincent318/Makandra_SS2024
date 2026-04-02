
namespace Replay.ViewModels.ProcessViewModels;

public class CreateTaskViewModel {
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int ProcessId { get; set; }
    public string[]? ContractTypes {get; set;}
    public string[]? Departments {get; set;}
    public string? TaskResponsible {get; set;}
    public string? TaskResponsibleRole {get; set;}
    public string[]? Roles {get; set;}
    public string? DueDateDefault {get; set;}
    public int? DueDateCounter {get; set;}
    public string? DueDateTimeUnit {get; set;}
    public bool? DueDateIsBefore {get; set;}
}
