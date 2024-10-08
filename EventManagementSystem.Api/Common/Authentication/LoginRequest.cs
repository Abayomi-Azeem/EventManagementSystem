﻿using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Api.Common.Authentication
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
