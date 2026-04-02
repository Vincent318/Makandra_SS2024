using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Replay.Models
{
    public class TaskBluePrint
    {
        [Key]
        public int Id { get; set; } 
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int ProcessId { get; set; } 
        public Process Process {get; set;}
        public DueDate DueDate {get; set;}
        public string? DepartmentsString {get; set;}
        public string? ContractTypesString {get; set;}
        public string? TaskResponsibleType {get; set;}
        public string? TaskResponsibleName {get; set;}

        [NotMapped]
        public string[]? Departments {
        get => DepartmentsString?.Split(',') ?? Array.Empty<string>();
        set {
            DepartmentsString = (value != null) ? string.Join(',', value) : null;
        } 
        }
        
        [NotMapped]
        public string[]? ContractTypes {
        get => ContractTypesString?.Split(',') ?? Array.Empty<string>();
        set {
            ContractTypesString = (value != null )? string.Join(',', value) : null ;
        } 
        }
        // [NotMapped]
        // public ITaskResponsible? TaskResponsible {
        // get => (ITaskResponsible) TaskRespnsibleRole ?? TaskResponsibleUser;
        // set
        // {
        //     if (value is User user) {
        //         TaskResponsibleUser = user;
        //         TaskRespnsibleRole = null;
        //     }
        //     else if (value is Role role) {
        //         TaskRespnsibleRole = role;
        //         TaskResponsibleUser = null;
        //     };
        // }
        
    }
}