using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusinessLayer.Interfaces;
using DataAccessLayer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTOs;
using ModelLayer.Models;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =====================
        //    REGISTER
        // =====================
        public AuthResponseDto Register(RegisterDto dto)
        {
            // 1. Check karo ki email already exist toh nahi karti
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Email already registered.");

            // 2. Salt generate karo (random 32 bytes)
            string salt = GenerateSalt();

            // 3. Password ko salt ke saath hash karo
            string hashedPassword = HashPassword(dto.Password, salt);

            // 4. Naya User object banao
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Salt = salt,
                HashedPassword = hashedPassword
            };

            // 5. DB mein save karo
            _context.Users.Add(user);
            _context.SaveChanges();

            // 6. JWT token generate karo aur return karo
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                Message = "Registration successful!"
            };
        }

        // =====================
        //    LOGIN
        // =====================
        public AuthResponseDto Login(LoginDto dto)
        {
            // 1. Email se user dhundo DB mein
            var user = _context.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
                throw new Exception("Invalid email or password.");

            // 2. Incoming password ko STORED salt ke saath hash karo
            string hashedAttempt = HashPassword(dto.Password, user.Salt);

            // 3. Compare karo stored hash se
            if (hashedAttempt != user.HashedPassword)
                throw new Exception("Invalid email or password.");

            // 4. JWT token generate karo aur return karo
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                Message = "Login successful!"
            };
        }

        // =====================
        //    HELPER: Salt banao
        // =====================
        private string GenerateSalt()
        {
            // 32 random bytes generate karo — ye har user ka alag hoga
            byte[] saltBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(saltBytes);
        }

        // =====================
        //    HELPER: Password hash karo
        // =====================
        private string HashPassword(string password, string salt)
        {
            // Salt ko bytes mein convert karo
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Password ko bytes mein convert karo
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Password + Salt ko combine karo
            byte[] combined = passwordBytes.Concat(saltBytes).ToArray();

            // SHA512 se hash karo
            byte[] hashBytes = SHA512.HashData(combined);

            return Convert.ToBase64String(hashBytes);
        }

        // =====================
        //    HELPER: JWT Token banao
        // =====================
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            // Token mein user ki info daalo (claims)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FirstName", user.FirstName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}