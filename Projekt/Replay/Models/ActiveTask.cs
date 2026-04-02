// @author: Raphael Huber

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Replay.Models
{
    /// <summary>
    /// The model for all tasks that are part of operations
    /// </summary>
    /// <author>Raphael Huber</author>
    public class ActiveTask
    {
        /// <summary>The auto-generated key used as an identifier for the database</summary>
        /// <author>Raphael Huber</author> 
        [Key]
        public int Id {get; set;}
        /// <summary>The title for the operation</summary>
        /// <author>Raphael Huber</author>
        public string? Title {get; set;}
        /// <summary>The instruction on how the tasks can be completed</summary>
        /// <author>Raphael Huber</author>
        public string? Instruction {get; set;} 
        /// <summary>The date when the task has to be completed at </summary>
        /// <author>Raphael Huber</author>
	    public DateOnly TargetDate {get; set;} 
        /// <summary>Defines if the task has been completed, started or is in progress</summary>
        /// <author>Raphael Huber</author>
	    public string? StatusString {get; set;}
        /// <summary>Usef for storing the ContractTypes-Array by storing all of them inside a single string</summary>
        /// <author>Raphael Huber</author>
        public string? ContractTypeString {get; set;}
        /// <summary>Usef for storing the Departments-Array by storing all of them inside a single string</summary>
        /// <author>Raphael Huber</author>
        public string? DepartmentsString {get; set;}
        /// <summary>Each task has a reponsible task owner
        /// When it is a role then this name is the title of the a role as this is unique
        /// When it is a user then this name is the email of the user because that defines him/summary>
        /// <author>Raphael Huber</author>
        public string? ResponsibleTaskOwnerName {get; set;}
        /// <summary>Each task has a reponsible task owner
        /// When it is a role this type is "role", when it is a user then "user"
        /// <author>Raphael Huber</author>
        public string? ResponsibleTaskOwnerType {get; set;}
        /// <summary>The Id of the parent operation, used for the 1:n-relationship</summary>
        /// <author>Raphael Huber</author>
        public int OperationId {get; set;}
        /// <summary>The parent operation, used for the 1:n-relationship</summary>
        /// <author>Raphael Huber</author>
        public Operation? Operation {get; set;}
        /// <summary>tasks can optionally limit for which contract types they have to be completed for
        /// f.e. the SSH-Keys only have to be created for employees that work in Development or Operations 
        /// This variable contains an array of the names of those contract types</summary>
        /// <author>Raphael Huber</author>
        [NotMapped]
        public string[] ContractTypes {
            get => ContractTypeString?.Split(',') ?? Array.Empty<string>();
            set => ContractTypeString = string.Join(',', value);
        }
        /// <summary>tasks can optionally limit for which departments they have to be completed for
        /// This variable contains an array of the names of those departments</summary>
        /// <author>Raphael Huber</author>
        [NotMapped]
        public string[] Departments {
            get => DepartmentsString?.Split(',') ?? Array.Empty<string>();
            set => DepartmentsString = string.Join(',', value);
        }
    }
}