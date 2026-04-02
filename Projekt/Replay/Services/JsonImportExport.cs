using System.IO;
using Replay.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;
using Replay.Models;
using Replay.Enumerations;
using System.Security.Cryptography.X509Certificates;
using Replay.Data;
using Microsoft.AspNetCore.Identity;

namespace Replay.Services;

    /// <summary>
    /// This class gives functionality to export and import all current data inside json-files
    /// </summary>
    /// <author>Raphael Huber</author>
    public class JsonImportExport
    {

        /// <summary>A operation-repository-instance to load and store all operations</summary>
        /// <author>Raphael Huber</author>
        private readonly OperationRepository _operationRepository;
        /// <summary>A process-repository-instance to load and store all proccesses</summary>
        /// <author>Raphael Huber</author>
        private readonly ProcessRepository _processRepository;
        /// <summary>A role-manager-instance to load and store all roles</summary>
        /// <author>Raphael Huber</author>
        private readonly RoleManager<Role> _roleManager;
        /// <summary>A user-manager-instance to load and store all users</summary>
        /// <author>Raphael Huber</author>
        private readonly UserManager<User> _userManager;
        /// <summary>A security-repository used for storing uid-email pairs</summary>
        /// <author>Raphael Huber</author>
        private readonly SecurityRepository _securityRepository;

        /// <summary>
        /// The constructor of the class
        /// </summary>
        /// <param name="opRep">value for _operationRepository</param>
        /// <param name="userManager">value for _userManager</param>
        /// <param name="roleManager">value for _roleManager</param>
        /// <param name="procRep"value for _processRepository</param>
        /// <author>Raphael Huber</author>
        public JsonImportExport(OperationRepository opRep, ProcessRepository procRep, RoleManager<Role> roleManager, UserManager<User> userManager, SecurityRepository securityRep)
        {
            _operationRepository = opRep;
            _processRepository = procRep;
            _roleManager = roleManager;
            _userManager = userManager;
            _securityRepository = securityRep;
        }

        /// <summary>
        /// Imports all the data that is stored in a "initial_data.json"-file if it exists
        /// </summary>
        /// <author>Raphael Huber</author>
        public async void importData()
        {
            var path = Path.Join(Environment.CurrentDirectory, "initial_data.json");

            if (!File.Exists(path)){
                return;
            }
            
            JsonSerializerOptions options = new()
            {
                //ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
                TypeInfoResolver = new JsonIgnorePropertyResolver()
            };

            string jsonString = File.ReadAllText(path);
            Wrapper? wrapper = JsonSerializer.Deserialize<Wrapper>(jsonString, options);

            if (wrapper is null) {
                return;
            }

            if (wrapper.operations is not null) {
                foreach (var op in wrapper.operations) {
                    if (!_operationRepository.OperationExists(op)) {
                        if (op.Tasks is not null) {
                            foreach (var task in op.Tasks) {
                                task.OperationId = op.Id;
                                task.Operation = op;
                            }
                        }
                        _operationRepository.Create(op);
                    }
                }
            }

            if (wrapper.roles is not null) {
                foreach (var role in wrapper.roles) {
                    if (_roleManager.FindByNameAsync(role.Title) is not null) {
                        Role newRole = new() {Name = role.Title, Description = role.Description, Title = role.Title};
                        await _roleManager.CreateAsync(newRole);
                    }
                }
            }

            if (wrapper.user is not null) {
                for (int i = 0; i < wrapper.user.Count; i++) {
                    var user = wrapper.user[i].user;
                    var password = wrapper.user[i].Password;
                    var locked = wrapper.user[i].IsLocked;

                    user.UserName = user.Email;

                    if (await _userManager.FindByEmailAsync(user.Email) is not null) {
                        continue;
                    }
                    var res = await _userManager.CreateAsync(user); 
                     

                    if (res.Succeeded) {
                        if (user.Roles[0] != "") {
                            await _userManager.AddToRolesAsync(user, user.Roles);
                        }
                    if (locked) {
                        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                        await _userManager.SetLockoutEnabledAsync(user, true);
                    }
                    SecurityModel securityModel = new() {Id = Guid.NewGuid(), Email = user.Email};
                    _securityRepository.Create(securityModel);

                    await _userManager.AddPasswordAsync(user, password);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, token);
                    _securityRepository.Delete(user.Email);
                }
                }
            }

            if (wrapper.processes is not null) {
                foreach (var proc in wrapper.processes) {
                    if (_processRepository.ProcessExists(proc)) {
                        continue;
                    }
                    if (proc.Tasks is not null) {
                        foreach (var task in proc.Tasks) {
                            task.ProcessId = proc.Id;
                            task.Process = proc;
                        }    
                    }
                    _processRepository.CreateProcess(proc);
                }
            }

            

        }

        /// <summary>
        /// Exports all operations, user, roles and processes into a export_data.json should it exist
        /// </summary>
        /// <author>Raphael Huber</author>
        public async void exportData()
        {
            var path = Path.Join(Environment.CurrentDirectory, "export_data.json");

            if (!File.Exists(path)){
                return;
            }

            JsonSerializerOptions options = new()
            {
                //ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
                TypeInfoResolver = new JsonIgnorePropertyResolver()
            };


            List<UserWrapper> user = new();
            foreach(var us in _userManager.Users.ToList()) {
                user.Add(new() {user = us, Password = "", IsLocked = await _userManager.IsLockedOutAsync(us)});
            }

            Wrapper content = new() {
                operations = _operationRepository.ReadAll(),
                processes = await _processRepository.LoadAllProcesses(),
                roles = _roleManager.Roles.ToList(),
                user = user
            };

            string jsonString = JsonSerializer.Serialize(content, options);
            File.WriteAllText(path, jsonString);

        }
    }

/// <summary>
/// This wrapper is used for storing all data so that a single Json-Exporter can be used 
/// -> All data will be stored in a single json-file
/// </summary>
/// <author>Raphael Huber</author>
internal class Wrapper
{
    public List<Operation>? operations { get; set; }
    public List<Process>? processes { get; set; }
    public List<Role>? roles { get; set; }
    public List<UserWrapper>? user { get; set; }
}

/// <summary>
/// The User doesn't store the password so this class is a wrapper to include it
/// It also has an attribute to lock the user out
/// </summary>
/// <author>Raphael Huber</author>
internal class UserWrapper
{
    public User? user {get; set;}
    public string? Password { get; set; }
    public bool IsLocked {get; set;}
}