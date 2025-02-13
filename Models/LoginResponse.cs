﻿namespace RoleBasedUserManagementApi.Models
{
    public class LoginResponse : Status
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? Expiration { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
