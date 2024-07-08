using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityManager.ServerAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShareClassLibrary.Contracts;
using ShareClassLibrary.Dtos;
using static ShareClassLibrary.Dtos.ServiceResponses;

namespace IdentityManager.ServerAPI.Repositories
{
    public class AccountRepository : IUserAccount
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AccountRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }


        public async Task<GeneralResponse> CreateAccountAsync(UserDto userDto)
        {
            if(userDto is null) return new GeneralResponse(false, "User model is empty");

            var newUser = new AppUser() {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                UserName = userDto.Email
            };
            
            // verify if user already exists
            var existingUser = await userManager.FindByEmailAsync(newUser.Email);
            if(existingUser is not null) return new GeneralResponse(false, "User already registered");

            // create new user
            var createdUser = await userManager.CreateAsync(newUser, userDto.Password);
            if(!createdUser.Succeeded) return new GeneralResponse(false, $"An error occured: {createdUser.Errors}.");
            
            // asign admin role to first user, rest as regular users
            var checkAdmin = await roleManager.FindByNameAsync("Admin");
            if(checkAdmin is null) 
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = "Admin"});
                await userManager.AddToRoleAsync(newUser, "Admin");
                return new GeneralResponse(true, "Account created successfully");
            }

            var checkUser = await roleManager.FindByNameAsync("User");
            if(checkUser is null)
            {
                await roleManager.CreateAsync(new IdentityRole(){Name = "User"});
            }
            await userManager.AddToRoleAsync(newUser, "User");
            return new GeneralResponse(true, "Account created successfully");
        }

        public async Task<LoginResponse> LoginAccountAsync(LoginDto loginDto)
        {
            if(loginDto is null) 
                return new LoginResponse(false, null!, "Login model is empty");

            var userFound = await userManager.FindByEmailAsync(loginDto.Email);
            if(userFound is null)
                return new LoginResponse(false, null!, "User not found, try login");

            var ckeckUserPassword = await userManager.CheckPasswordAsync(userFound, loginDto.Password);
            if(!ckeckUserPassword)
                return new LoginResponse(false, null!, "Invalid email or password");
            
            var userRoles = await userManager.GetRolesAsync(userFound);
            var userSession = new UserSession(userFound.Id, userFound.Name, userFound.Email, userRoles.First());

            var token = GenerateToken(userSession);
            return new LoginResponse(true, token, "Login successful");
        }

        private string GenerateToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var token = new JwtSecurityToken(
                issuer: this.configuration["Jwt:Issuer"],
                audience: this.configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}