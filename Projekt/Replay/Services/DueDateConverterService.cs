

using System.Text;
using Replay.Models;

namespace Replay.Services;
/// <author>Daniel Feustel</author>
public class DueDateConverterService {

    public static DateOnly Convert(DueDate dueDateModel, DateOnly dueDate){

        DateOnly newDate = new(dueDate.Year, dueDate.Month, dueDate.Day);
        if (dueDateModel.DefaultValue != null) {
            switch (dueDateModel.DefaultValue) {
                case "ASAP":
                    newDate = DateOnly.FromDateTime(DateTime.Now); 
                    break;
                case "2 Wochen vor Start":
                    newDate.AddDays(-14);
                    break;
                case "Am ersten Arbeitstag":
                    // No Action required: newDate = ProcessDuedate
                    break;
                case "3 Wochen nach Arbeitsbeginn":
                    newDate.AddDays(21);
                    break;
                case "3 Monate nach Arbeitsbeginn":
                    newDate.AddMonths(3);
                    break;
                case "6 Monate nach Arbeitsbeginn":
                    newDate.AddMonths(6);
                    break;
            }
        } else {
            
            int sign = dueDateModel.IsBefore ? -1 : 1;

            switch (dueDateModel .TimeUnit) {   
                case "Tag(e)":
                    newDate.AddDays(dueDateModel.Counter * sign);
                    break;
                case "Woche(n)":
                    newDate.AddDays(dueDateModel.Counter * 7 * sign);
                    break;
                case "Monat(e)":
                    newDate.AddMonths(dueDateModel.Counter * sign);
                    break;
                case "Jahr(e)":
                    newDate.AddYears(dueDateModel.Counter * sign);
                    break;
            }
            
        }   
        return newDate;
    }

        

        

}