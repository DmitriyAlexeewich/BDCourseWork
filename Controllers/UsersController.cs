using DataLayer.DataBaseUpdater.DataContext;
using DataLayer.Models.Models;
using Dto.Abstract.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TradeManagementAPI.Controllers
{
    [Authorize(Roles = "SystemAdmin")] // Только системный администратор
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public UsersController(PostgreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<IEnumerable<User>>>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return ObjectResultDto<IEnumerable<User>>.Ok(users, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<IEnumerable<User>>.Error($"Error retrieving users: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<User>>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return ObjectResultDto<User>.Error("User not found");

                return ObjectResultDto<User>.Ok(user, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<User>.Error($"Error retrieving user: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return ObjectResultDto<bool>.Error("User not found", false);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return ObjectResultDto<bool>.Ok(true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error($"Error deleting user: {ex.Message}", false);
            }
        }
    }
}
