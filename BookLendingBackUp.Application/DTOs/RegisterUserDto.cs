using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookLendingBackUp.Application.DTOs
{
    public class RegisterUserDto
    {
        
            [JsonPropertyName("fullName")]
            public string FullName { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }

            [JsonPropertyName("role")]
            public string Role { get; set; }
    }


    }

