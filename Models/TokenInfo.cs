namespace RoleBasedUserManagementApi.Models
{
    public class TokenInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
