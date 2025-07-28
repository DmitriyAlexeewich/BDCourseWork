using DataLayer.DataAccess.Context;
using DataLayer.DataBaseUpdater.DataContext;
using DataLayer.Models.Models;
using Dto.Abstract.Result;
using Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TradeManagementAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly PostgreDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(PostgreDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ObjectResultDto<User>>> Register(UserRegistrationDto registrationDto)
        {
            try
            {
                // Проверка существования пользователя
                if (await _context.Users.AnyAsync(u => u.Username == registrationDto.Username))
                    return ObjectResultDto<User>.Error("Username already exists");

                // Проверка ролевых ограничений
                string error = ValidateRoleConstraints(registrationDto);
                if (!string.IsNullOrEmpty(error))
                    return ObjectResultDto<User>.Error(error);

                // Проверка существования привязанных сущностей
                error = await ValidateEntityExistence(registrationDto);
                if (!string.IsNullOrEmpty(error))
                    return ObjectResultDto<User>.Error(error);

                var user = new User
                {
                    Username = registrationDto.Username,
                    Role = registrationDto.Role,
                    StoreNumber = registrationDto.StoreNumber,
                    DepartmentName = registrationDto.DepartmentName,
                    WarehouseName = registrationDto.WarehouseName
                };

                // Хеширование пароля
                user.PasswordHash = _passwordHasher.HashPassword(user, registrationDto.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Загружаем связанные сущности для ответа
                await LoadRelatedEntities(user);

                return ObjectResultDto<User>.Ok(user, "User registered successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<User>.Error($"Error registering user: {ex.Message}");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ObjectResultDto<string>>> Login(UserLoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (user == null)
                    return ObjectResultDto<string>.Error("Invalid username or password");

                // Проверка пароля
                var result = _passwordHasher.VerifyHashedPassword(
                    user, user.PasswordHash, loginDto.Password);

                if (result == PasswordVerificationResult.Failed)
                    return ObjectResultDto<string>.Error("Invalid username or password");

                var token = GenerateJwtToken(user);

                return ObjectResultDto<string>.Ok(token, "Login successful");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<string>.Error($"Error during login: {ex.Message}");
            }
        }

        private string ValidateRoleConstraints(UserRegistrationDto dto)
        {
            switch (dto.Role)
            {
                case "StoreAdmin" when !dto.StoreNumber.HasValue:
                    return "Store number is required for Store Admin role";

                case "DepartmentHead" when !dto.StoreNumber.HasValue || string.IsNullOrEmpty(dto.DepartmentName):
                    return "Store number and department name are required for Department Head role";

                case "WarehouseAdmin" when string.IsNullOrEmpty(dto.WarehouseName):
                    return "Warehouse name is required for Warehouse Admin role";

                default:
                    return string.Empty;
            }
        }

        private async Task<string> ValidateEntityExistence(UserRegistrationDto dto)
        {
            if(dto.Role == "SystemAdmin")
                return string.Empty;

            if (dto.StoreNumber.HasValue &&
                !await _context.Stores.AnyAsync(s => s.Number == dto.StoreNumber))
                return "Store not found";

            if (!string.IsNullOrEmpty(dto.DepartmentName) && dto.StoreNumber.HasValue &&
                !await _context.Departments.AnyAsync(d =>
                    d.StoreNumber == dto.StoreNumber && d.Name == dto.DepartmentName))
                return "Department not found";

            if (!string.IsNullOrEmpty(dto.WarehouseName) &&
                !await _context.Warehouses.AnyAsync(w => w.Name == dto.WarehouseName))
                return "Warehouse not found";

            return string.Empty;
        }

        private async Task LoadRelatedEntities(User user)
        {
            if (user.StoreNumber.HasValue)
                user.Store = await _context.Stores.FindAsync(user.StoreNumber);

            if (user.StoreNumber.HasValue && !string.IsNullOrEmpty(user.DepartmentName))
                user.Department = await _context.Departments.FindAsync(
                    user.StoreNumber, user.DepartmentName);

            if (!string.IsNullOrEmpty(user.WarehouseName))
                user.Warehouse = await _context.Warehouses.FindAsync(user.WarehouseName);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role)
            };

            // Добавляем дополнительные данные в зависимости от роли
            if (user.StoreNumber.HasValue)
                claims.Add(new("StoreNumber", user.StoreNumber.ToString()!));

            if (!string.IsNullOrEmpty(user.DepartmentName))
                claims.Add(new("DepartmentName", user.DepartmentName));

            if (!string.IsNullOrEmpty(user.WarehouseName))
                claims.Add(new("WarehouseName", user.WarehouseName));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
