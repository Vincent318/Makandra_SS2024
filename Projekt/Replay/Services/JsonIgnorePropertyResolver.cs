using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Replay.Models;

namespace Replay.Services;

/// <summary>
/// A class for removing unneccessary properties of the classes for the json-class
/// </summary>
/// <author>Raphael Huber</author>
public class JsonIgnorePropertyResolver : DefaultJsonTypeInfoResolver
{
    /// <summary>
    /// Used to remove all unneceserry class-attributes for the json import/export
    /// </summary>
    /// <param name="type">This parameter is used to specify the type of object that the JsonTypeInfo should be returned for</param>
    /// <param name="options">This parameter is used to customize the serialization process</param>
    /// <returns>The JsonTypeInfo returned by this method is used by the JSON serializer to understand how to serialize and deserialize instances of the specified type</returns>
    /// <author>Raphael Huber</author>
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo typeInfo = base.GetTypeInfo(type, options);

        if (typeInfo.Kind == JsonTypeInfoKind.Object && type == typeof(User)) {
	        foreach (var prop in new List<string>() {"PhoneNumber", "DepartmentsString", "RolesString", "LockoutEnabled", "AccessFailedCount", "LockoutEnd", "TwoFactorEnabled", "Id", "UserName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "PhoneNumberConfirmed", "EmailConfirmed", "ConcurrencyStamp"}) {
                var ignoreProperty = typeInfo.Properties.FirstOrDefault(p => p.Name == prop);
                typeInfo.Properties.Remove(ignoreProperty);
            }
        } else if (typeInfo.Kind == JsonTypeInfoKind.Object && type == typeof(Role)) {
            foreach (var prop in new List<string>() {"Id", "TasksResponsibleString", "Name", "NormalizedName", "ConcurrencyStamp"}) {
                var ignoreProperty = typeInfo.Properties.FirstOrDefault(p => p.Name == prop);
                typeInfo.Properties.Remove(ignoreProperty);
            }
        } else if (typeInfo.Kind == JsonTypeInfoKind.Object && type == typeof(ActiveTask)) {
            foreach (var prop in new List<string>() {"ContractTypeString", "DepartmentsString", "OperationId", "Operation"}) {
                var ignoreProperty = typeInfo.Properties.FirstOrDefault(p => p.Name == prop);
                typeInfo.Properties.Remove(ignoreProperty);
            }
        } else if (typeInfo.Kind == JsonTypeInfoKind.Object && type == typeof(Operation)) {
            foreach (var prop in new List<string>() {"ReferencePerson", "PersonInCharge"}) {
                var ignoreProperty = typeInfo.Properties.FirstOrDefault(p => p.Name == prop);
                typeInfo.Properties.Remove(ignoreProperty);
            }
        } else if (typeInfo.Kind == JsonTypeInfoKind.Object && type == typeof(Process)) {
            foreach (var prop in new List<string>() {"AuthorizedRolesString"}) {
                var ignoreProperty = typeInfo.Properties.FirstOrDefault(p => p.Name == prop);
                typeInfo.Properties.Remove(ignoreProperty);
            }

        } else if (typeInfo.Kind == JsonTypeInfoKind.Object && type == typeof(TaskBluePrint)) {
            foreach (var prop in new List<string>() {"ProcessId", "Process", "DepartmentsString", "ContractTypesString"}) {
                var ignoreProperty = typeInfo.Properties.FirstOrDefault(p => p.Name == prop);
                typeInfo.Properties.Remove(ignoreProperty);
            }
        }

        return typeInfo;
    }
}