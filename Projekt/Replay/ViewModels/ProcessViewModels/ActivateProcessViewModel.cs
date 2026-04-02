using Replay.Models;

namespace Replay.ViewModels.ProcessViewModels;

public class ActivateProcessViewModel {

    public string? Title {get; set;}
    public string? Description {get; set;}
    public Process? Process {get; set;}
    public int ProcessId {get; set;}
    public User? ReferencePerson {get; set;}
    public User? PersonInCharge {get; set;}
    public string? ReferencePersonEmail {get; set;}
    public string? PersonInChargeEmail {get; set;}
    public bool Archived {get; set;}
    public DateOnly TargetDate {get; set;}
    public string TargetDateString {get; set;}
    public List<User>? Users {get; set;}
    public string? Department {get; set;}
    public string? ContractType {get; set;}
    public string? CurrentUserEmail {get; set;}

}
