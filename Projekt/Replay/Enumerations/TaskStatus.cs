using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace Replay.Enumerations;

/// <summary> <defines the current status of tasks from operations
/// can be in finished, in progress or not started
/// </summary>
/// <author>Raphael Huber</author>
public enum TaskStatus {
	UNBEARBEITET,
	BEARBEITET,
	IN_BEARBEITUNG
}
