namespace RoleBasedUserManagementApi.Models
{
    public class TokenResponse
    {
        public string? Token { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
