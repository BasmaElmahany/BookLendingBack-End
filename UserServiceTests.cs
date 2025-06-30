using AutoMapper;
using BookLendingBackUp.Application.DTOs;
using BookLendingBackUp.Application.Interfaces;
using BookLendingBackUp.Application.Settings;
using BookLendingBackUp.Infrastructure.Entities;
using BookLendingBackUp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingBackUp.Tests
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly IEmailSender _emailSender;
        private readonly UserService _userService;

        public UserServiceTests()
        {

            var store = Substitute.For<IUserStore<ApplicationUser>>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>(store, null, null, null, null, null, null, null, null);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterUserDto, ApplicationUser>();
            });
            _mapper = config.CreateMapper();


            _jwtOptions = Options.Create(new JwtSettings
            {
                Key = "SuperSecureJWTKeyWithLength1234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                DurationInMinutes = 60
            });
            
            _userService = new UserService(_userManager, _mapper, _jwtOptions, _emailSender);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccessMessage_WhenUserIsCreated()
        {

            var dto = new RegisterUserDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Test@123"
            };

            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
                .Returns(IdentityResult.Success);


            var result = await _userService.RegisterAsync(dto);


            Assert.Equal("User registered successfully", result);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserCreationFails()
        {

            var dto = new RegisterUserDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Test@123"
            };

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Email already taken" });
            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
                .Returns(identityResult);


            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.RegisterAsync(dto));
            Assert.Contains("Email already taken", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@example.com",
                FullName = "Test User",
                UserName = "test@example.com"
            };

            var dto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            _userManager.FindByEmailAsync(dto.Email).Returns(user);
            _userManager.CheckPasswordAsync(user, dto.Password).Returns(true);


            var token = await _userService.LoginAsync(dto);


            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenCredentialsAreInvalid()
        {

            var dto = new LoginDto
            {
                Email = "invalid@example.com",
                Password = "wrongpass"
            };

            _userManager.FindByEmailAsync(dto.Email).Returns((ApplicationUser)null);


            var ex = await Assert.ThrowsAsync<Exception>(() => _userService.LoginAsync(dto));
            Assert.Equal("Invalid login", ex.Message);
        }
    }
}
