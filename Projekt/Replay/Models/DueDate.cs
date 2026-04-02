using System.ComponentModel.DataAnnotations;

namespace Replay.Models;
/// <author>Daniel Feustel</author>
public class DueDate {

    [Key]
    public int Id {get; set;}
    public string? DefaultValue {get; set;}
    public int Counter {get; set;}
    public string TimeUnit {get; set;}
    public bool IsBefore {get; set;}
    public string? Title {get; set;}
    
}