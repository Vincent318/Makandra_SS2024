// @author: Daniel Feustel
using Replay.Enumerations;

namespace Replay.Services;
public class EnumMapperService {

    public static string GetDepartmentName(Department department) {
        return department switch
        {
            Department.BACKOFFICE => "Backoffice",
            Department.ENTWICKLUNG => "Entwicklung",
            Department.OPERATIONS => "Operations",
            Department.PROJEKTMANAGEMENT => "Projektmanagement",
            Department.SALES => "Sales",
            Department.UIUX => "UI/UX",
            _ => "",
        };
    }

    public static string GetTaskStatusName(Enumerations.TaskStatus status) {
        return status switch
        {
            Enumerations.TaskStatus.UNBEARBEITET => "unbearbeitet",
            Enumerations.TaskStatus.BEARBEITET => "bearbeitet",
            Enumerations.TaskStatus.IN_BEARBEITUNG => "in Bearbeitung",
            _ => "",
        };
    }

    public static Enumerations.TaskStatus GetTaskStatus(string status) {
       
        return status switch
        {
            "unbearbeitet" => Enumerations.TaskStatus.UNBEARBEITET,
            "bearbeitet" => Enumerations.TaskStatus.BEARBEITET,
            "in Bearbeitung" => Enumerations.TaskStatus.IN_BEARBEITUNG,
            _ => Enumerations.TaskStatus.UNBEARBEITET,
        };
    }

    public static string GetContractType(ContractType contract) {
        return contract switch
        {
            ContractType.WERKSTUDENT => "Werkstudent",
            ContractType.FESTANSTELLUNG => "Festanstellung",
            ContractType.PRAKTIKUM => "Praktikum",
            ContractType.TRAINEE => "Trainee",
            _ => "",
        };
    }

    public static string GetDueDate(DueDateEnum dueDate) {
        return dueDate switch {
            DueDateEnum.ASAP => "ASAP",
            DueDateEnum.DREI_MONATE_NACH_START => "3 Monate nach Arbeitsbeginn",
            DueDateEnum.DREI_WOCHEN_NACH_START => "3 Wochen nach Arbeitsbeginn",
            DueDateEnum.ERSTER_ARBEITSTAG => "Am ersten Arbeitstag",
            DueDateEnum.SECHS_MONATE_NACH_START => "6 Monate nach Arbeitsbeginn",
            DueDateEnum.ZWEI_WOCHEN_VOR_START => "2 Wochen vor Start",
            _ => ""
        };
    }

}
