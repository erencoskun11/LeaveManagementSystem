using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services; 
using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;

        private readonly INotificationService _notificationService;

        public AuthService(
            IEmployeeRepository employeeRepository,
            IConfiguration configuration,
            INotificationService notificationService)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
            _notificationService = notificationService;
        }

        private string GenerateJwtToken(Employee employee)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Role, employee.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiresMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Employee> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser = await _employeeRepository.GetByEmailAsync(request.Email);
            if (existingUser != null) throw new Exception("Bu e-posta adresi zaten kayıtlı.");

            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                AnnualLeaveAllowance = request.AnnualLeaveAllowance,
                Role = request.Role,
                DepartmentId = request.DepartmentId,

                HireDate = DateTime.SpecifyKind(request.HireDate, DateTimeKind.Utc),
                CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                IsDeleted = false
            };

            await _employeeRepository.AddAsync(employee);

            await _notificationService.SendEmployeeUpdate();

            return employee;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var employee = await _employeeRepository.GetByEmailAsync(request.Email);

            if (employee == null || employee.Password != request.Password)
            {
                throw new Exception("E-posta veya şifre hatalı.");
            }

            var token = GenerateJwtToken(employee);

            return new LoginResponseDto
            {
                Token = token,
                Email = employee.Email,
                Role = employee.Role.ToString()
            };
        }

        public async Task<Role> GetUserRoleAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            if (employee == null) throw new Exception("Kullanıcı bulunamadı.");
            return employee.Role;
        }
    }
}