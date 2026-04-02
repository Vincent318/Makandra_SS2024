using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Replay.Models;


namespace Replay.Repositories
{
    /// <summary>
    /// Repository class for managing users, mostly to filter out locked users
    /// </summary>
    /// <author> Robet Figl </author>
    public class UserRepository
    {
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// constructor to get the user manager
        /// </summary>
        /// <param name="userManager">Manages Users</param>
        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Uses user manager to get all users and the filters them on only the ones not locked
        /// </summary>
        /// <returns>List of users not locked</returns>
        public async Task<List<User>> GetActiveUsersAsync()
        {
            // Gets all users
            var users = await _userManager.Users.ToListAsync();
            // returns only the ones not locked
            return users.Where(u => !u.LockoutEnabled || (u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.Now)).ToList();
        }
    }
}