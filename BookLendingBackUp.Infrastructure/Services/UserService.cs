using AutoMapper;
using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Application.Settings;
using BookLendingBackUp.Infrastructure.Entities;
using BookLendingBackUp.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailSender _emailSender;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper, IOptions<JwtSettings> jwtSettings , IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _emailSender = emailSender;
        }

        //public async Task<string> RegisterAsync(RegisterUserDto dto)
        //{
        //    var user = _mapper.Map<ApplicationUser>(dto);
        //    user.UserName = dto.Email;

        //    var result = await _userManager.CreateAsync(user, dto.Password);
        //    if (!result.Succeeded)
        //        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        //    return "User registered successfully";
        //}
        public async Task<string> RegisterAsync(RegisterUserDto dto)
        {
            var user = _mapper.Map<ApplicationUser>(dto);
            user.UserName = dto.Email;
            user.DesiredRole = dto.Role; // Save the chosen role

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmationLink = $"http://localhost:12957/api/Auth/confirmemail?userId={user.Id}&token={encodedToken}";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>");

            return "User registered successfully. Please check your email to confirm.";
        }


        public async Task<string> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Invalid user ID");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                throw new Exception("Email confirmation failed");

            // Assign role if not already assigned
            if (!await _userManager.IsInRoleAsync(user, user.DesiredRole))
            {
                var roleExists = await _userManager.AddToRoleAsync(user, user.DesiredRole);
                if (!roleExists.Succeeded)
                {
                    throw new Exception("Failed to assign role.");
                }
            }

            return "Email confirmed and role assigned.";
        }


        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new Exception("Invalid credentials");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new Exception("Please confirm your email first.");

            return await GenerateJwtToken(user);
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = await GetUserClaimsAsync(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
                {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.FullName ?? user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
         

            var isMember = await _userManager.IsInRoleAsync(user, "Member");
            if (isMember)
                claims.Add(new Claim(ClaimTypes.Role, "Member"));
            return claims;
        }



    }
}
