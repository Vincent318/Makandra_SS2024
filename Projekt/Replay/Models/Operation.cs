// @author: Raphael Huber

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Replay.Models
{
    /// <summary>
    /// The model for Operations that are active instances of processes
    /// </summary>
    /// <author>Raphael Huber</author>
    public class Operation
    {

        /// <summary>The auto-generated key used as an identifier for the database</summary>
        /// <author>Raphael Huber</author> 
        [Key]
        public int Id {get; set;}
        /// <summary>A name/title the operation should have</summary>
        /// <author>Raphael Huber</author> 
        public string? Title {get; set;}
        /// <summary>A description for the operation</summary>
        /// <author>Raphael Huber</author> 
        public string? Description {get; set;} 
        /// <summary>The date where operation should be finished at</summary>
        /// <author>Raphael Huber</author> 
	    public DateOnly TargetDate {get; set;} 
        /// <summary>The contract type that should be created by the process</summary>
        /// <author>Raphael Huber</author> 
        public string? ContractTypeString {get; set;}
        /// <summary>The department that the person should have at the end of the process</summary>
        /// <author>Raphael Huber</author> 
        public string? DepartmentString {get; set;}  
        /// <summary>The person that the operation references (f.e. the new employee) </summary>
        /// <author>Raphael Huber</author> 
        [NotMapped]  
        public User? ReferencePerson {get; set;}
        /// <summary>The person that the is in charge of the operation</summary>
        /// <author>Raphael Huber</author>
        [NotMapped]
        public User? PersonInCharge {get; set;} 
        /// <summary>The mail of the person that the operation references
        /// Is used as a key in the database</summary>
        /// <author>Raphael Huber</author>
        public string? PersonInChargeMail {get; set;}
        /// <summary>The mail of the person that the is in charge of the operation
        /// Is used as a key in the database</summary>
        /// <author>Raphael Huber</author>
        public string? ReferencePersonMail {get; set;}
        /// <summary>All active tasks that the operation has</summary>
        /// <author>Raphael Huber</author>
	    public List<ActiveTask>? Tasks {get; set;}
        /// <summary>Boolean on whether the operation has been archived or not</summary>
        /// <author>Raphael Huber</author>
        public bool Archived {get; set;}
    }

}